public interface IInputModel
{
	float GetAxis(string key);
	bool GetButtonDown(string key);
	bool GetButton(string key);
	bool GetButtonUp(string key);
}