from django.contrib.auth.models import User
from django.db import models


class User(User):
    game_build = models.PositiveIntegerField()
