using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public interface IHintSource
	{
		List<GeneratedHint> GetHints();
	}
}
