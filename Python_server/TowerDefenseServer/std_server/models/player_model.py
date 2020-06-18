from django.db import models

from .user_model import MyUser


class Player(models.Model):
    identity = models.ForeignKey(MyUser, on_delete=models.CASCADE, verbose_name="user", unique=True)
    player_address = models.CharField(max_length=128, unique=True)
    user_address = models.CharField(max_length=128, unique=True)
    level = models.PositiveIntegerField()
    points = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
