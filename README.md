# HololensJobFunctionApp1
Job_Function_app for 2019T1 comp6324 group7

Job_Function_app is a EventHubTrigger function respond to IoT hub. The application create a job and send to Job table in SQL database if the sensor data is abnormal. In this case , brightness less than 185 will be a fault and the application will create a job ticket if the faulty emergency light has no maintenance job.
