﻿// 
// General:
//      This file is part of .NET Bridge
//
// Copyright:
//      2010 Jonathan Shore
//      2017 Jonathan Shore and Contributors
//
// License:
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at:
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
//

using System;
using bridge.common.io;


namespace bridge.server.ctrl
{
	/// <summary>
	/// CLR Call Method message.
	/// </summary>
	public class CLRCallMethodMessage : CLRMessage
	{
		public CLRCallMethodMessage ()
			: base (TypeCallMethod)
		{
		}

		public CLRCallMethodMessage (object obj, string method, params object[] args)
			: base (TypeCallMethod)
		{
			Obj = obj;
			MethodName = method;
			Parameters = args;
		}


		// Properties

		public object Obj
			{ get; private set; }

		public string MethodName
			{ get; private set; }

		public object[] Parameters
			{ get; private set; }


		// Serialization
		
		/// <summary>
		/// Serialize the message.
		/// </summary>
		/// <param name="cout">Cout.</param>
		public override void Serialize (IBinaryWriter cout)
		{
			base.Serialize (cout);

			// class & method names
			cout.WriteInt32 (CLRObjectProxy.ProxyIdFor (Obj));
			cout.WriteString (MethodName);

			// arguments
			cout.WriteUInt16 ((ushort)Parameters.Length);
			for (int i = 0 ; i < Parameters.Length ; i++)
				CLRMessage.SerializeValue (cout, Parameters[i]);
		}

		
		/// <summary>
		/// Deserialize the message (assumes magic & type already read in)
		/// </summary>
		/// <param name="cin">Cin.</param>
		public override void Deserialize (IBinaryReader cin)
		{
			// class & method names
			Obj = CLRObjectProxy.Find (cin.ReadInt32(), proxyok: true);
			MethodName = cin.ReadString();

			// arguments
			var len = (int)cin.ReadUInt16 ();
			Parameters = new object[len];

			for (int i = 0 ; i < len ; i++)
				Parameters[i] = CLRMessage.DeserializeValue (cin);
		}
	}
}

