﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{

	[Serializable]
	public class DataAccessException : Exception
	{
		public DataAccessException() { }
		public DataAccessException(string message) : base(message) { }
		public DataAccessException(string message, Exception inner) : base(message, inner) { }
		protected DataAccessException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
