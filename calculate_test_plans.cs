using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.IO;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.4")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
    {
        // TODO : Add here the code that is called when the script is launched from Eclipse.
        var plans = context.Course.ExternalPlanSetups;
        string res = "";

            DateTime dt = DateTime.Now;
            string dt_str = dt.ToString("yyyyMMdd_HHmmss");
            string output_path = "\\\\aria\\VA_TRANSFER\\miyata_workdir\\esapi_dir\\testplan_calculation_log\\" + dt_str + ".txt";

            //MessageBox.Show(output_path);

            string output_dir = Path.GetDirectoryName(output_path);
            if (!string.IsNullOrEmpty(output_dir))
            {
                Directory.CreateDirectory(output_dir);
            }
            else {; }

            File.AppendAllText(output_path, $"Start calculation at {dt.ToString("yyyy/MM/dd HH:mm:ss")}\n\n");


            //var has_dose = context.ExternalPlanSetup.IsDoseValid;
            //MessageBox.Show(has_dose.ToString());

            res = $"Course: {context.Course.Id}\n";
            foreach (var plan in plans)
            {
                if (!plan.IsDoseValid)
                {
                    res += $"\tPlan: {plan.Id}\n";
                }
                else {; }
            }
            res += "Start calculations for plans above.\n";
            res += $"Log file is output to {output_path}.";
            res += $"\n\nDo you want to set field weights to 1.000?";
            var is_reset_weight = MessageBox.Show(res, "", MessageBoxButton.YesNoCancel);
            if (is_reset_weight == MessageBoxResult.Cancel) return;

            res = "";

            context.Patient.BeginModifications();
            foreach (var plan in plans)
            {
                if (plan.IsDoseValid)
                {
                    res += String.Format("Plan: {0} is skipped (it has valid dose.)\n", plan.Id);
                }
                else
                {
                    plan.SetCalculationOption(plan.PhotonCalculationModel, "FieldNormalizationType", "No field normalization"); // to avoid to show warning dialog indicating "dose at normalization point is too low. dose could not be normalized.". This dialog prevent to continue following calculations.
                    var preset_params_list = new List<BeamParameters>();

                    // set 100 MU for IMRT plan (it might not work because of the ESAPI bug)
                    List<KeyValuePair<string, MetersetValue>> preset_values = new List<KeyValuePair<string, MetersetValue>>();
                    foreach (var beam in plan.Beams)
                    {
                        preset_params_list.Add(beam.GetEditableParameters());

                        var meterset = new MetersetValue(100.0, DosimeterUnit.MU);
                        preset_values.Add(new KeyValuePair<string, MetersetValue>(beam.Id, meterset));
                    }
                    plan.CalculateDoseWithPresetValues(preset_values);

                    
                    foreach (var beam in plan.Beams.Select((x, i) => new {Value = x, Index = i}))
                    {
                        if (is_reset_weight == MessageBoxResult.Yes)
                        {
                            preset_params_list[beam.Index].WeightFactor = 1.0;
                        }
                        else {; }
                        beam.Value.ApplyParameters(preset_params_list[beam.Index]);
                    }


                    res += String.Format("Plan: {0} is calculated.\n\tAlgorithm: {1}", plan.Id, plan.PhotonCalculationModel);
                    foreach (var elem in plan.PhotonCalculationOptions)
                    {
                        res += String.Format("\n\t{0}: {1}", elem.Key, elem.Value);
                    }
                    res += "\n";
                }

                dt = DateTime.Now;
                dt_str = dt.ToString("yyyy/MM/dd HH:mm:ss");
                res = $"{dt_str}\n" + res;

                File.AppendAllText(output_path, res);
                res = "";
            }


        //MessageBox.Show(res);

    }
  }
}
