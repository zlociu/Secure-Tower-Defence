from django.db import models

# Create your models here.


class Test(models.Model):
    name = models.CharField(max_length=5, default="game", editable=False, unique=True)
    actual_build = models.PositiveIntegerField()

    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
