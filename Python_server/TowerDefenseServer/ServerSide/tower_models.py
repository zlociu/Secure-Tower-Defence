import json
from django.db import models


class Tower(models.Model):

    GRND = "GROUND"
    AIR = "FLYING"
    ALL = "ALL"
    NONE = "NONE"

    TOWER_TYPES = [
        (GRND, 1),
        (AIR, 2),
        (ALL, 3),
        (NONE, -1),
    ]

    identity = models.CharField(max_length=127, unique=True,primary_key=True)
    name = models.CharField(max_length=127)
    type = models.IntegerField(choices=TOWER_TYPES, default=ALL)
    level = models.PositiveIntegerField()
    attack_damage = models.PositiveIntegerField()
    fire_rate = models.FloatField()
    attack_distance = models.FloatField()
    price = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
