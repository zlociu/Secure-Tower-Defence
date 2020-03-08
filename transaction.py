from hashlib import sha256 
import json
import player

class Transaction:

    def __init__(self, player: str):
        """
        contains player converted to string
        """
        self.player = player
    
    def transactionJSON(self):
        trString = json.dumps(self.__dict__, sort_keys=True)
        return trString
    
    def compute_hash(self):
        tr_string = self.transactionJSON()
        return sha256(tr_string.encode()).hexdigest()

