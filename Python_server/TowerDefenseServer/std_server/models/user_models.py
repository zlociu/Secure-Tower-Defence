from django.contrib.auth.models import User
from django.db import models


class MyUser(User):
    game_build = models.PositiveIntegerField()
