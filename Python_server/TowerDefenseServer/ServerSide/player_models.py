from hashlib import sha256 
import json
from django.db import models
from django.contrib.auth.models import User


class Player(models.Model):
    identity = models.ForeignKey(User, on_delete=models.CASCADE, verbose_name="user", unique=True)
    player_address = models.CharField(max_length=127, unique=True)
    user_address = models.CharField(max_length=127, unique=True)
    level = models.PositiveIntegerField()
    points = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    def addPoints(self, value):
        if value > 0:
            points = points + value

    def increaseLevel():
        level = level + 1

    def description(self):
        txt = " id: {} \n lvl: {} \n points: {}"
        return txt.format(self.identity, self.lvl, self.points)

    def playerJSON(self):
        playerString = json.dumps(self.__dict__, sort_keys=True)
        return playerString

        




