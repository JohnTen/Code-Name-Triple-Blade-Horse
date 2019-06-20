using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class Transisition
{
	Transisition t = new Transisition(
		"Anim1",
		"Anim2",
		(b,f,i) => { return b["Jump"] && f["XSpeed"] > 0; }
		);

	Func<bool,
		Dictionary<string, bool>,
		Dictionary<string, float>,
		Dictionary<string, int>> rule;

	public Transisition(
		string animation1, 
		string animation2, 
		Func<bool, 
			Dictionary<string, bool>, 
			Dictionary<string, float>, 
			Dictionary<string, int>> rule)
	{
		this.rule = rule;
	}
	
	public bool Test()
	{
		return false;
	}
}
