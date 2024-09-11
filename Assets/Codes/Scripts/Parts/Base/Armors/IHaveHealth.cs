public interface IHealth
{
	/// <summary>
	/// The current health of the part.
	/// </summary>
	float Health { get; }

	/// <summary>
	/// The maximum health of the part.
	/// </summary>
	float MaxHealth { get; }

	bool IsDead { get; }

	/// <summary>
	/// When the part takes damage.
	/// </summary>
	/// <param name="damage"> The amount of damage to take.</param>
	void TakeDamage(float damage);

	/// <summary>
	/// When the part is healed.
	/// </summary>
	/// <param name="heal"> The amount of health to heal.</param>
	void Heal(float heal);

	void OnHealthDepleted();

	void Restore();
}