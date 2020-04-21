import json
from django.db import models

class Tower(models.Model):

    class TowerType(models.IntegerChoices):
        EARTH = 1
        FLY = 2
        ALL = 3
        NONE = -1


    identity = models.CharField(max_length=127, unique=True,primary_key=True)
    name = models.CharField(max_length=127)
    type = models.IntegerField(choices=TowerType.choices, default=TowerType.ALL)
    level = models.PositiveIntegerField()
    attack_damage = models.PositiveIntegerField()
    fire_rate = models.FloatField()
    attack_distance = models.FloatField()
    price = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


    @classmethod
    def create(cls, identity, name, type, level, attack_damage, attack_distance, fire_rate, price):

        tower = cls(
            identity = identity,
            name = name,
            type = type,
            level = level,
            attack_damage = attack_damage,
            attack_distance = attack_distance,
            fire_rate = fire_rate,
            price = price
        )
        return tower

    
    def update(self,data):
        pass