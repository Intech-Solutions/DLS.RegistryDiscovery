#region Using Namespaces

using System.Collections.Generic;

#endregion

public class Request
{
	#region Public Members

	public Header header;

	#endregion

	#region Internal Members

	private List<Question> questions;

	#endregion

	#region Constructors

	public Request()
	{
		header = new Header
		{
			QDCOUNT	= 0,
			OPCODE	= OPCode.Query
		};

		questions = new List<Question>();
	}

	#endregion

	#region Fields

	public byte[] Data
	{
		get
		{
			List<byte> data	= new List<byte>();
			header.QDCOUNT	= (ushort)questions.Count;
			data.AddRange(header.Data);

			foreach (Question q in questions)
				data.AddRange(q.Data);
			
			return data.ToArray();
		}
	}

	#endregion

	#region Public Methods

	public void AddQuestion(Question question)
	{
		questions.Add(question);
	}

    #endregion
}