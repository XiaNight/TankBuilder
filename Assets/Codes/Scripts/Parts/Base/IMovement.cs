public interface IMovement
{
	/// <summary>
	/// Set the acceleration level of the motors, from -1 to 1
	/// </summary>
	/// <param name="amount">float value -1 to 1</param>
	void SetTorque(float amount);

	/// <summary>
	/// Set the brake force of the motors, from 0 to 1
	/// </summary>
	/// <param name="amount">float value 0 to 1</param>
	void SetBrake(float amount);

	void SetSteer(float amount);

	float UnitRPM { get; }
}