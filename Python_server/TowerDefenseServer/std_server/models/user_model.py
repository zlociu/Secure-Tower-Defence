from django.contrib.auth.models import User
from django.db import models


class User(User):
    version = models.PositiveIntegerField(default=0)
