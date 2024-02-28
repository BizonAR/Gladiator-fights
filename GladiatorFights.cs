using System;
using System.Collections.Generic;

namespace GladiatorFights
{
	internal class GladiatorFights
	{
		static void Main()
		{
			Arena arena = new Arena();
			arena.Work();
		}
	}
}

class Arena
{
	private const string CommandFight = "1";
	private const string CommandExit = "2";

	private List<Fighter> _fighters;
	private Fighter _firstFighter;
	private Fighter _secondFighter;

	public Arena()
	{
		_fighters = new List<Fighter>
		{
			new Warrior("Воин", 150, 30),
			new Mage("Маг", 100, 20, 150),
			new Rogue("Разбойник", 80, 40),
			new Paladin("Паладин", 120, 20),
			new Healer("Целитель", 120, 5)
		};
	}

	public void Work()
	{
		bool isProgramActive = true;

		while (isProgramActive)
		{
			Console.WriteLine("Список команд:\n" +
					$"{CommandFight} - бой между 2-мя бойцами\n" +
					$"{CommandExit} - выход из программы");

			Console.Write("Введите команду: ");
			string input = Console.ReadLine();

			switch (input)
			{
				case CommandFight:
					Fight();
					break;

				case CommandExit:
					isProgramActive = false;
					break;

				default:
					Console.WriteLine("Неизвестная команда!");
					break;
			}
			Console.Write("Нажмите любую кнопку чтобы продолжить: ");
			Console.ReadKey();
			Console.Clear();
		}
	}

	public void Fight()
	{
		if (TryChooseFighters())
		{
			while (_firstFighter.Health > 0 && _secondFighter.Health > 0)
			{
				_firstFighter.ShowStats();
				_secondFighter.ShowStats();
				_firstFighter.Attack(_secondFighter);
				_secondFighter.Attack(_firstFighter);
			}

			ShowFightResult();
		}
	}

	private Fighter ChooseFighter(string number)
	{
		Fighter fighter = null;
		Console.Write($"Выберите {number} бойца: ");

		if (int.TryParse(Console.ReadLine(), out int index))
		{
			if (index > 0 && index < _fighters.Count + 1)
				fighter = _fighters[index - 1].Clone();
			else
				Console.WriteLine("Не правильно выбран номер");
		}
		else
		{
			Console.WriteLine("Вы ввели не число");
		}
		return fighter;
	}

	private void ShowFightResult()
	{
		if (_firstFighter.Health <= 0 && _secondFighter.Health <= 0)
			Console.WriteLine("Ничья");
		else if (_firstFighter.Health > 0)
			Console.WriteLine("Победил первый боец");
		else
			Console.WriteLine("Победил второй боец");
	}

	private void ShowFighters()
	{
		for (int i = 0; i < _fighters.Count; i++)
		{
			Console.Write(i + 1 + " ");
			_fighters[i].ShowStats();
		}
	}

	private bool TryChooseFighters()
	{
		ShowFighters();
		_firstFighter = ChooseFighter("первого");

		if (_firstFighter == null)
		{
			Console.WriteLine("Ошибка в выборе бойцов");
			return false;
		}

		_secondFighter = ChooseFighter("второго");

		if (_secondFighter == null)
		{
			Console.WriteLine("Ошибка в выборе бойцов");
			return false;
		}

		Console.WriteLine("Бойцы выбраны");
		return true;
	}
}

abstract class Fighter
{
	public Fighter(string name, int health, int damage)
	{
		Name = name;
		Health = health;
		Damage = damage;
	}

	public string Name { get; protected set; }
	public int Health { get; protected set; }
	public int Damage { get; protected set; }

	public virtual void TakeDamage(int damage)
	{
		Health -= damage;
	}

	public abstract void Attack(Fighter opponent);

	public abstract Fighter Clone();

	public void ShowStats()
	{
		Console.WriteLine($"{Name} - {Health} здоровья, {Damage} - урон");
	}
}

class Warrior : Fighter
{
	public Warrior(string name, int health, int damage) : base(name, health, damage) { }

	public override void Attack(Fighter opponent)
	{
		opponent.TakeDamage(Damage);
	}

	public override Fighter Clone()
	{
		return new Warrior(Name, Health, Damage);
	}
}

class Mage : Fighter
{
	private int _attackManaCost = 20;
	private int _maximumMana;
	private int _currentMana;
	private double _manaRestorePercentage = 0.1;

	public Mage(string name, int health, int damage, int maximumMana) : base(name, health, damage)
	{
		_maximumMana = maximumMana;
		_currentMana = maximumMana;
	}

	public override void Attack(Fighter opponent)
	{
		if (_currentMana > _attackManaCost)
		{
			opponent.TakeDamage(Damage);
			_currentMana -= _attackManaCost;
		}
		else
		{
			Console.WriteLine($"{Name} не хватает маны для атаки");
		}

		RestoreMana();
	}

	public override Fighter Clone()
	{
		return new Mage(Name, Health, Damage, _maximumMana);
	}

	private void RestoreMana()
	{
		int manaToRestore = (int)(_maximumMana * _manaRestorePercentage);
		_currentMana = Math.Min(_maximumMana, _currentMana + manaToRestore);
	}
}

class Rogue : Fighter
{
	private int _attackCount = 0;
	private int _criticalHitFrequency = 3;
	private int _criticalHitMultiplier = 2;

	public Rogue(string name, int health, int damage) : base(name, health, damage) { }

	public override void Attack(Fighter opponent)
	{
		_attackCount++;

		if (_attackCount % _criticalHitFrequency == 0)
		{
			Console.WriteLine($"{Name} наносит критический удар по {opponent.Name}, нанося урон {Damage}!");
			opponent.TakeDamage(Damage * _criticalHitMultiplier);
		}
		else
		{
			opponent.TakeDamage(Damage);
		}
	}

	public override Fighter Clone()
	{
		return new Rogue(Name, Health, Damage);
	}
}

class Paladin : Fighter
{
	private double _chanceToDodge = 0.3;

	private Random _random = new Random();

	public Paladin(string name, int health, int damage) : base(name, health, damage) { }

	public override void Attack(Fighter opponent)
	{
		opponent.TakeDamage(Damage);
	}

	public override void TakeDamage(int damage)
	{
		if (_random.NextDouble() < _chanceToDodge)
			Console.WriteLine($"{Name} увернулся от атаки");
		else
			base.TakeDamage(Damage);
	}

	public override Fighter Clone()
	{
		return new Paladin(Name, Health, Damage);
	}
}

class Healer : Fighter
{
	private int _healingPerTurn = 10;
	private int _maximumHealth;

	public Healer(string name, int health, int damage) : base(name, health, damage)
	{
		_maximumHealth = Health;
	}

	public override void Attack(Fighter opponent)
	{
		opponent.TakeDamage(Damage);
	}

	public override void TakeDamage(int damage)
	{
		base.TakeDamage(damage);
		Heal();
	}

	public override Fighter Clone()
	{
		return new Healer(Name, Health, Damage);
	}

	private void Heal()
	{
		if (Health < _maximumHealth)
		{
			Health = Math.Min(_maximumHealth, Health + _healingPerTurn);
			Console.WriteLine($"{Name} вылечил HP на {_healingPerTurn}. Текущее здоровье: {Health}");
		}
	}
}