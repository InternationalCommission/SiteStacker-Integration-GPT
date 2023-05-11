import os
from dotenv import load_dotenv

# Load environment variables from .env file
load_dotenv()

# SiteStacker API credentials
SS_API_ID = os.getenv("SS_API_ID")
SS_API_SECRET = os.getenv("SS_API_SECRET")

# Constant Contact API credentials
CC_CLIENT_ID = os.getenv("CC_CLIENT_ID")
CC_CLIENT_SECRET = os.getenv("CC_CLIENT_SECRET")
CC_REFRESH_TOKEN = os.getenv("CC_REFRESH_TOKEN")

# Constant Contact API scopes
CC_SCOPES = ["contact_data"]  # modify this as needed

# General settings
SYNC_MODE = "full"  # options are "full" and "incremental"
