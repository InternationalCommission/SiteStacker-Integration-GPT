class SSAPIError(Exception):
    """Raised for errors in SiteStacker API"""

    def __init__(self, message, response=None):
        super().__init__(message)
        self.response = response


class CCServerError(Exception):
    """Raised for errors in Constant Contact API"""

    def __init__(self, message, response=None):
        super().__init__(message)
        self.response = response


class SyncError(Exception):
    """Raised for errors during synchronization"""

    def __init__(self, message):
        super().__init__(message)
