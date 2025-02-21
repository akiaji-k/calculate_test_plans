# calculate_test_plans_esapi_v16_1

ESAPI Binary Plugin to calculate all plans visualize beam dose rate.

Created with ESAPI v16.1.

<span style="color:#ff0000;">This is in the process of i8n.</span>



## How to use

1. Running the script will display the un-calculated plan and the path to a text file that outputs the progress of the calculations.

   ![select_target](./images/calculate_test_plans.png)

2. Clicking the button shown below will perform dose calculations for all uncalculated plans in the current course. Dose calculation parameters must be set to appropriate values prior to the calculation (e.g., dose calculation algorithm).

   - Yes -> Calculates the dose, and the field weights of all fields are set to 1.000.
   - No -> Calculates the dose, and the field weights of all fields are kept at the values before the script is executed.
   - Cancel -> No dose calculation is executed.

3. During script execution, the calculation log file continues to be updated.

   ![select_target](./images/calculation_log.png)



## LICENSE

Released under the MIT license.

No responsibility is assumed for anything that occurs with this software.

See [LICENSE](https://github.com/akiaji-k/plan_checker_gui_esapi_v15_5/blob/main/LICENSE) for further details.