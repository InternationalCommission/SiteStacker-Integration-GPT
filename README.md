SiteStacker-Integration-GPT
This project provides a Python script to synchronize contact records between SiteStacker and Constant Contact. It is designed to be run daily on a cron job, once the application has been authenticated the first time.

Prerequisites
Before running the script, make sure you have the following:

Python 3.x installed on your machine
Access to the SiteStacker and Constant Contact APIs
API keys and authentication details for both SiteStacker and Constant Contact
Getting started
To get started with this project, follow these steps:

Clone the repository to your local machine using Git
Create a config.py file in the root directory of the project with your API keys and authentication details. See config_sample.py for an example.
Install any required Python packages by running pip install -r requirements.txt
Run the synchronize.py script using python synchronize.py
Configuration
The config.py file should contain the following information:

SS_API_URL: The base URL for the SiteStacker API
SS_API_KEY: Your SiteStacker API key
SS_API_SECRET: Your SiteStacker API secret key
CC_API_KEY: Your Constant Contact API key
CC_API_SECRET: Your Constant Contact API secret key
CC_REDIRECT_URI: The redirect URI for the Constant Contact oAuth2 Device Flow
CC_ACCESS_TOKEN: Your Constant Contact access token
CC_REFRESH_TOKEN: Your Constant Contact refresh token
LOG_FILE: The file path for the log file
Troubleshooting
If you encounter any issues while running the script, please check the following:

Make sure your API keys and authentication details are correct
Check that you have installed all required Python packages
Verify that your network connection is working correctly
Check the log file for any error messages or exceptions
License
This project is licensed under the GPL 3.0 license. See the LICENSE file for more information.

Disclaimer
This project is not affiliated with SiteStacker or Constant Contact in any way. Use at your own risk.
