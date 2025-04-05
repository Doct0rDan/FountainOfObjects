Gamefield newGamefield = new Gamefield(3,3);
Player player = new Player(newGamefield);
newGamefield.GamefieldUpdate(player);
while (newGamefield.gameWon == false)
{
    newGamefield.Update(player);
}