from hashlib import sha256 
import json
from django.db import models

class Player:

class Player(models.Model):
    identity = models.ForeignKey(User, on_delete=models.CASCADE, verbose_name="user", unique=True)
    player_address = models.CharField(max_length=127, unique=True)
    user_address = models.CharField(max_length=127, unique=True)
    level = models.PositiveIntegerField()
    points = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    @classmethod
    def create(cls, identity, player_address, user_address):
        """
        Constructor
        # id = unique number, will be public key
        # login = user's name
        # password = password hashed with SHA256
        """
        player = cls(   identity=identity,
                        player_address=player_address,
                        user_address = user_address)

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

        




