using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public interface IHandler<C>
{
	HandleResult HandleRequest(C condition);

	event Action<HandleResult> OnHandled;
}

public interface IHandler<C1, C2> : IHandler<C1>
{
	HandleResult HandleRequest(C1 condition1, C2 condition2);
}

public interface IHandler<C1, C2, C3> : IHandler<C1, C2>
{
	HandleResult HandleRequest(C1 condition1, C2 condition2, C3 condition3);
}