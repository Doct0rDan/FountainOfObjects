using System.Diagnostics;
public class Gamefield
{
    public Room?[,] newGamefield {get; protected set;}
    public bool fountainActive {get; protected set; } = false;
    private Room fountainRoom;
    private Room emptyRoom = new EmptyRoom();
    public Room entrance = new Entrance();
    public IMove[] moves = new IMove[1];
    public bool gameWon { get; private set; } = false;


    public Gamefield(int Rows, int Cols)
    {
        Random random = new Random();
        newGamefield = new Room[Rows, Cols];
        fountainRoom = new FountainRoom(newGamefield);

        newGamefield[fountainRoom.Row, fountainRoom.Col] = fountainRoom;
        newGamefield[entrance.Row, entrance.Col] = entrance;
        for (int i = 0; i < newGamefield.GetLength(0); i++)
        {
            for (int j = 0; j < newGamefield.GetLength(1); j++)
            {
                if (newGamefield[i,j] != entrance && newGamefield[i,j] != fountainRoom)
                {
                    if (random.Next(0,100) < 33)
                    {
                        newGamefield[i,j] = new EnemyRoom(i,j);
                    }
                    else
                    {
                        newGamefield[i,j] = new EmptyRoom();
                    }
                }
            }
        }
    }
    public void Update(Player player)
    {
        FountainSound(player);
        MovePlayer(player);
        GamefieldUpdate(player);
        CheckEnemy(player);
        CheckWinCondition(player);
    }
    public void CheckEnemy(Player player)
    {
        for (int i = 0; i < newGamefield?.GetLength(0); i++)
        {
            for (int j = 0; j < newGamefield?.GetLength(1); j++)
            {
                if (player.Row == i && player.Col == j && newGamefield[i,j]?.RoomContents == RoomContents.Enemy_Battle)
                {
                    EnemyEncounter Encounter;
                    Encounter = new EnemyEncounter(player);
                    Encounter.Start(player, this);
                    newGamefield[i,j].RoomContents = RoomContents.Enemy_Dead;
                }
            }
        }

    }
    public void MovePlayer(Player player)
    {
        for (int i = 0; i < this.moves.Length; i++)
        {
            string? inst = Console.ReadLine();
            IMove command = inst switch
            {
                "north" => new NorthMove(),
                "south" => new SouthMove(),
                "east" => new EastMove(),
                "west" => new WestMove(),
                _ => new NoMove(),
            };
            this.moves[i] = command;
        }
        foreach (var move in moves)
        {
            move.Move(player, this);
        }
    }
    public void GamefieldUpdate(Player player)
    {
        for (int i = 0; i < newGamefield?.GetLength(0); i++)
            {
                Console.WriteLine("–––––––––––––––––––––––––––––––––––––––––-");
                for (int j = 0; j < newGamefield?.GetLength(1); j++)
                {
                    if (player.Row == i && player.Col == j)
                    {
                        Console.Write($"| {"(⛹)", -10}");
                      //Console.Write($"| {"(X)", -10}");
                        newGamefield[i,j].HasVisited = true;
                    }
                    else
                    {
                        if (newGamefield[i,j]?.HasVisited == true)
                        {
                            Console.Write($"| {newGamefield?[i, j]?.ContentsToString(), -10}");
                        }
                        else
                        {
                            Console.Write($"| {newGamefield?[i, j]?.Name, -10}");
                        }
                    }
                    Console.Write(" |");
                }
                Console.WriteLine();
            }
            if (player.Col == this.fountainRoom.Col && player.Row == this.fountainRoom.Row)
            {
                fountainActive = true;
                this.fountainRoom.RoomContents = RoomContents.Activated;
                AudioMaker.PlaySound(Path.Combine("Assets", "Elden Ring 'Lost Grace'.mp3"));
                Console.WriteLine("The Fountain has been activated!");
            }

    }
    public void CheckWinCondition(Player player)
    {
        if (player.CheckDeath(this) || CheckPitFalled(player))
        {
            Console.WriteLine("You Lost!");
            Environment.Exit(0);
        }
        if (fountainActive == true && player.Row == this.entrance.Row && player.Col == this.entrance.Col)
        {
            Console.WriteLine("YOU WON!");
            gameWon = true;
        } 
    }

    public bool CheckPitFalled(Player player)
    {
        for (int i = 0; i < newGamefield?.GetLength(0); i++)
        {
            for (int j = 0; j < newGamefield?.GetLength(1); j++)
            {
                if (this.newGamefield[i, j]?.HasEntered(player) == true && this.newGamefield[i, j]?.RoomContents == RoomContents.Pitfall)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void FountainSound(Player player)
    {
        if (this.fountainActive == true)
        {
            return;
        }
        //FROM PLAYER PERSPECTIVE!
        char NorthSouth = ' ';
        char EastWest = ' ';
        //Check Vertical Axis
        if (player.Row < this.fountainRoom.Row)
        {
            NorthSouth = 'S';
        }
        else if (player.Row > this.fountainRoom.Row)
        {
            NorthSouth = 'N';
        }
        else
        {
            NorthSouth = ' ';
        }
        //Check Horizontal Axis
        if (player.Col < this.fountainRoom.Col)
        {
            EastWest = 'E';
        }
        else if (player.Col > this.fountainRoom.Col)
        {
            EastWest = 'W';
        }
        else
        {
            EastWest = ' ';
        }
        Console.WriteLine($"You hear a dripping coming from {NorthSouth}{EastWest}. That must be the fountain!");
    }
        private void MonsterSound(Player player)
    {
        //FROM PLAYER PERSPECTIVE!
        char NorthSouth = ' ';
        char EastWest = ' ';
        //Check Vertical Axis
        for (int i = 0; i < this.newGamefield.GetLength(0); i++)
        {
            for (int j = 0; j < this.newGamefield.GetLength(1); i++)
            {
                if (player.Row < this.fountainRoom.Row)
                {
                    NorthSouth = 'S';
                }
                else if (player.Row > this.fountainRoom.Row)
                {
                    NorthSouth = 'N';
                }
                else
                {
                    NorthSouth = ' ';
                }
                //Check Horizontal Axis
                if (player.Col < this.fountainRoom.Col)
                {
                    EastWest = 'E';
                }
                else if (player.Col > this.fountainRoom.Col)
                {
                    EastWest = 'W';
                }
                else
                {
                    EastWest = ' ';
                }
            }
        }

        Console.WriteLine($"You hear a dripping coming from {NorthSouth}{EastWest}. That must be the fountain!");
    }
    public void PrintGamefield()
    {
        for (int i = 0; i < newGamefield?.GetLength(0); i++)
        {
            Console.WriteLine("––––––––––––––––––––––––––––––––––––––––––");
            for (int j = 0; j < newGamefield?.GetLength(1); j++)
            {
                Console.Write($"| {newGamefield[i,j]?.ContentsToString(), -10} |");
            }
            Console.WriteLine();
        }
    }
}
public abstract class Room
{
    public abstract int Row { get; set; }
    public abstract int Col { get; set; }
    public string Name { get; protected set; } = "???";
    public bool playerInside = false;
    public bool HasVisited { get; set; } = false;
    public bool HasEntered(Player player)
    {
        if (player.Row == this.Row && player.Col == this.Col)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public abstract RoomContents RoomContents{ get; set; }

    public string ContentsToString()
    {
        return this.RoomContents switch
        {
            RoomContents.Empty => "   ",
            RoomContents.Activated => "☽☉☾",
            RoomContents.Entrance => "Entrance",
            RoomContents.Fountain => "Fountain",
            RoomContents.Enemy_Battle => "  ",
            RoomContents.Enemy_Dead => "☠ Enemy ☠",
            _ => "   "
        };
    }
}
public class EmptyRoom : Room
{
    public override int Row { get; set; }
    public override int Col { get; set; }
    public override RoomContents RoomContents{ get; set;} = RoomContents.Empty;
}
public class FountainRoom : Room
{
    public override int Row { get; set; }
    public override int Col { get; set; }
    public override RoomContents RoomContents{ get; set; }

    public bool activated { get; set; } = false;
    public FountainRoom(Room?[,] newGamefield)
    {
        Random random = new Random();
        do
        {
            Row = random.Next(0, newGamefield.GetLength(0));
            Col = random.Next(0, newGamefield.GetLength(1));
        }while (Row == 0 && Col == 0);
        RoomContents = RoomContents.Fountain;
    }  
}
public class Entrance : Room
{
    public override int Row { get; set; }
    public override int Col { get; set; }
    public override RoomContents RoomContents { get; set; }
    public Entrance()
    {
        Row = 0;
        Col = 0;
        RoomContents = RoomContents.Entrance;
    }
}
public class EnemyRoom : Room
{
    public override int Row { get; set; }
    public override int Col { get; set; }
    public override RoomContents RoomContents { get; set; }

    public EnemyRoom(int row, int col)
    {
        Row = row;
        Col = col;
        RoomContents = RoomContents.Enemy_Battle;
    }
}
public class PitRoom : Room
{
    public override int Row { get; set; }
    public override int Col { get; set; }
    public override RoomContents RoomContents {get; set; }

    public PitRoom(int row, int col)
    {
        Row = row;
        Col = col;
        RoomContents = RoomContents.Pitfall;
    }

}
public class Player
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int Level { get; private set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public int XP { get; set; } = 0;
    public bool alive { get; set; } = true;
    public Room currentRoom { get; set; }
    List<IAttack> attacks { get; set; } = new List<IAttack>();
    public IAttack[] AP = new IAttack[1];
    public Player(Gamefield gamefield)
    {
        currentRoom = gamefield.entrance;
        Row = gamefield.entrance.Row;
        Col = gamefield.entrance.Col;
        Level = 1;
        HP = 10;
        MP = 10;
        attacks.Add(new BaseAttack());
        attacks.Add(new NoAttack());
        attacks.Add(new SMITE());
    }

    public void PlayerAttacks(Enemy enemy)
    {
        int queueSize = this.AP.Length;
        this.AP = new IAttack[this.AP.Length];
        for (int i = 0; i < this.AP.Length; i++)
        {
           
            Console.WriteLine("Queue an Attack. Remaining Queue Size:" + queueSize);
            string? inst = Console.ReadLine();
            IAttack command = inst switch
            {
                "base attack" => new BaseAttack(),
                "SMITE" => new SMITE(),
                "nothing" => new NoAttack(),
                _ => new NoAttack(),
            };
            if (attacks != null && attacks.Any(a => a.GetType() == command.GetType()))
            {
            this.AP[i] = command;
            queueSize--;
            Console.WriteLine(command + "Has been Queued. Remaining Queue Size:"+ queueSize);
            }
            else
            {
                Console.WriteLine("Unknown Move");
                this.AP[i] = new NoAttack();
            }
        }
        foreach (var move in this.AP)
        {
            move.Attack(this, enemy);
        }
    }
    public bool CheckDeath(Gamefield gamefield)
    {
        if (this.HP <= 0 || gamefield.CheckPitFalled(this))
        {
            Console.WriteLine("YOU DIED");
            AudioMaker.PlaySound(Path.Combine("Assets", "Dark Souls 'YOU DIED'.mp3"));
            return true;
        }
        else
        {
            return false;
        }
    }

}
public interface IMove
{
    void Move(Player player, Gamefield gamefield);
    bool ValidMove(Player player, Gamefield gamefield, int Row, int Col)
    {
        if  (player.Row + Row < 0 || player.Col + Col < 0 || player.Row + Row >= gamefield.newGamefield.GetLength(0) || player.Col + Col >= gamefield.newGamefield.GetLength(1) )
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
public class NoMove : IMove
{
    public void Move(Player player, Gamefield gamefield)
    {
        Console.WriteLine("Nothing Happened");
        return;
    }
}
public class SouthMove : IMove
{
    public void Move(Player player, Gamefield gamefield)
    {
        if (((IMove)this).ValidMove(player, gamefield, 1, 0) == true)
        {
            player.Row += 1;
            foreach (Room? room in gamefield.newGamefield)
            {
                if (room?.Row == player.Row && room.Col == player.Col)
                {
                    player.currentRoom = room;
                }
            }            
        }
        else
        {
            Console.WriteLine("Invalid Move!");
        }

    }
}
public class NorthMove : IMove
{
    public void Move(Player player, Gamefield gamefield)
    {
        if (((IMove)this).ValidMove(player, gamefield, -1, 0) == true)
        {
            player.Row += -1;
            foreach (Room? room in gamefield.newGamefield)
            {
                if (room?.Row == player.Row && room.Col == player.Col)
                {
                    player.currentRoom = room;
                }
            }            
        }
        else
        {
            Console.WriteLine("Invalid Move!");
        }
    }
}
public class WestMove : IMove
{
    public void Move(Player player, Gamefield gamefield)
    {
        if (((IMove)this).ValidMove(player, gamefield, 0, -1) == true)
        {
            player.Col += -1;
            foreach (Room? room in gamefield.newGamefield)
            {
                if (room?.Row == player.Row && room.Col == player.Col)
                {
                    player.currentRoom = room;
                }
            }            
        }
        else
        {
            Console.WriteLine("Invalid Move!");
        }
    }
}
public class EastMove : IMove
{
    public void Move(Player player, Gamefield gamefield)
    {
        if (((IMove)this).ValidMove(player, gamefield, 0, 1) == true)
        {
            player.Col += 1;
            foreach (Room? room in gamefield.newGamefield)
            {
                if (room?.Row == player.Row && room.Col == player.Col)
                {
                    player.currentRoom = room;
                }
            }            
        }
        else
        {
            Console.WriteLine("Invalid Move!");
        }
    }
}
public class EnemyEncounter
{
    int EncounterLevel { get; }
    public Enemy enemy { get; protected set; }
    public bool isOver = true;
    public EnemyEncounter(Player player)
    {
        isOver = false;
        Console.WriteLine("An Enemy will arrive shortly. Please remain seated.");
        AudioMaker.PlaySound(Path.Combine("Assets", "Pokémon - Encounter Start.mp3"));
        enemy = Enemy.CreateEnemy(player);
        Console.WriteLine($"A WILD {enemy.Name} APPEARS!");
    }
    public void Start(Player player, Gamefield gamefield)
    {
        while (isOver == false)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"You have {player.HP}HP. What would you like to do? Enemy HP: {enemy.HP}");
            player.PlayerAttacks(enemy);
            if (enemy.HP > 0)
            {
                Console.WriteLine($"Enemy has {enemy.HP} remaining");
                Console.WriteLine("ENEMY ATTACKS!");
                new BaseEnemyAttack().Attack(player,enemy);
            }

            if (player.HP <= 0)
            {
                player.CheckDeath(gamefield);
                isOver = true;
                Environment.Exit(0);
            }
            if (enemy.HP <= 0)
            {
                Console.WriteLine("Enemy defeated!!!!!!!!");
                isOver = true;
                break;
            }
        }

    }
}
public class Enemy
{
    public int Level { get; protected set;}
    public string? Name { get; protected set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public Enemy(Player player)
    {
        Level = 1;
        Name = "Hollow Soul";
        HP = 1;
        MP = 1;
    }
    public static Enemy CreateEnemy(Player player)
    {
        Array alleGegnerTypen = Enum.GetValues(typeof(EnemyID));
        Random random = new Random();
        EnemyID randomEnemyID = (EnemyID)alleGegnerTypen.GetValue(random.Next(alleGegnerTypen.Length));
        Enemy enemy = new Zombie(player);
        if (Enum.IsDefined(typeof(EnemyID), randomEnemyID))
        {
            enemy = randomEnemyID switch
                {
                    EnemyID.Ghoul => new Ghoul(player),
                    EnemyID.Zombie => new Zombie(player),
                    _ => new Ghoul(player),
                };
        }
        return enemy;
    }
}

public class Ghoul : Enemy
{
    public Ghoul(Player player) : base(player)
    {
        Name = "Ghoul";
        Level = player.Level;
        HP = player.HP + 5;
        MP = HP;
    }
}

public class Zombie : Enemy
{
    public Zombie (Player player) : base(player)
    {
        Name = "Zombie";
        Level = player.Level;
        HP = player.HP + 10;
        MP = 0;
    }
}
public interface IAttack
{
    void Attack(Player player, Enemy enemy);
}
public class BaseEnemyAttack : IAttack
{
    public void Attack(Player player, Enemy enemy)
    {
        int damage = enemy.Level + 20;
        player.HP -= damage;
    }
}
public class BaseAttack : IAttack
{
    public void Attack (Player player, Enemy enemy)
    {
        int damage = player.Level + 3;
        enemy.HP -= damage;
    }
}
public class SMITE : IAttack
{
    public void Attack (Player player, Enemy enemy)
    {
        int damage = 100;
        enemy.HP -= damage;
    }
}

public class NoAttack : IAttack
{
    public void Attack(Player player, Enemy enemy)
    {
        int damage = 0;
        Console.WriteLine("Nothing happened.");
    }
}
public class AudioMaker
{
    public static void PlaySound(string location)
    {
        string audioFile = location; // Ersetze mit deinem Dateipfad

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "afplay",
                Arguments = $"\"{audioFile}\"",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit(); // Warten, bis das Audio zu Ende ist
    }
}