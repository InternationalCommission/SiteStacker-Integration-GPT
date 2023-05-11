# SiteStacker-Integration-GPT

This is a Python script that synchronizes contact records between the SiteStacker and Constant Contact platforms.

## Getting Started

1. Clone this repository to your local machine.
2. Install the required Python packages by running `pip install -r requirements.txt`.
3. Configure the settings in the `config.py` file.
4. Run the script using the command `python main.py`.

## Configuration

The `config.py` file contains configuration settings for the project. You will need to set the following variables:

* `ss_api_id`: Your SiteStacker API ID.
* `ss_api_secret`: Your SiteStacker API secret.
* `ss_base_url`: The base URL for your SiteStacker API.
* `cc_client_id`: Your Constant Contact client ID.
* `cc_client_secret`: Your Constant Contact client secret.
* `cc_redirect_uri`: The redirect URI for your Constant Contact application.
* `cc_scopes`: The scopes that your Constant Contact application requires.
* `cc_base_url`: The base URL for the Constant Contact API.

## License

This project is licensed under the GPL 3.0 license. See the `LICENSE` file for details.
