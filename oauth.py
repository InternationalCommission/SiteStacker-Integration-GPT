import os
import webbrowser
import json
import time
import requests
from requests.auth import HTTPBasicAuth
from urllib.parse import urlencode

from constants import CC_SCOPES


def get_authorization_url(client_id, redirect_uri, scopes=CC_SCOPES):
    """Returns the authorization URL for the Constant Contact API"""
    base_url = 'https://api.cc.email/v3/idfed'
    params = {
        'client_id': client_id,
        'redirect_uri': redirect_uri,
        'response_type': 'device',
        'scope': ' '.join(scopes)
    }
    url = base_url + '?' + urlencode(params)
    return url


def get_access_token(client_id, client_secret, device_code):
    """Gets the access token using the device flow"""
    base_url = 'https://idfed.constantcontact.com/as/token.oauth2'
    payload = {
        'grant_type': 'urn:ietf:params:oauth:grant-type:device_code',
        'device_code': device_code
    }
    response = requests.post(
        url=base_url,
        data=payload,
        auth=HTTPBasicAuth(client_id, client_secret)
    )
    response.raise_for_status()
    return response.json()['access_token']


def authenticate(client_id, client_secret, redirect_uri):
    """Authenticates the application using the device flow"""
    auth_url = get_authorization_url(client_id, redirect_uri)
    print('Please go to the following URL and grant access:')
    print(auth_url)

    # Open the URL in the default browser
    webbrowser.open(auth_url)

    device_code = input('Enter the device code displayed on the page: ')
    access_token = None
    while access_token is None:
        try:
            access_token = get_access_token(client_id, client_secret, device_code)
        except requests.exceptions.HTTPError as e:
            # If the authorization is still pending, wait for a few seconds before trying again
            if e.response.status_code == 400 and e.response.json().get('error') == 'authorization_pending':
                time.sleep(5)
            else:
                raise e
    print('Authentication successful!')
    return access_token


def refresh_access_token(client_id, client_secret, refresh_token):
    """Refreshes the access token using the refresh token"""
    base_url = 'https://idfed.constantcontact.com/as/token.oauth2'
    payload = {
        'grant_type': 'refresh_token',
        'refresh_token': refresh_token
    }
    response = requests.post(
        url=base_url,
        data=payload,
        auth=HTTPBasicAuth(client_id, client_secret)
    )
    response.raise_for_status()
    return response.json()['access_token']
