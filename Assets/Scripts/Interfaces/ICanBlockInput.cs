using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICanBlockInput
{
	bool BlockInput { get; }
	bool DelayInput { get; }
}
