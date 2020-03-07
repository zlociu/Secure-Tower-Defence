from hashlib import sha256 
import json

class Transaction:

    def __init__(self, player):
        self.player = player.playerJSON()
    
    def transactionJSON(self):
        trString = json.dumps(self.__dict__, sort_keys=True)
        return trString
    
    def compute_hash(self):
        tr_string = self.transactionJSON()
        return sha256(tr_string.encode()).hexdigest()

