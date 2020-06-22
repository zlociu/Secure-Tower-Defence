from django.db import models

# Create your models here.


class GameBuild(models.Model):
    name = models.CharField(max_length=5, default="game", editable=False, unique=True)
    version = models.PositiveIntegerField(default=0)

    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
