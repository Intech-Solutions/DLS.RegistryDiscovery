#region Using Namespaces

using System;
using System.Net;
using System.Collections.Generic;

#endregion

public class Response
{
	#region Public Members

	/// <summary>
	/// List of AdditionalRR records
	/// </summary>
	public List<AdditionalRR> Additionals;

	/// <summary>
	/// List of AnswerRR records
	/// </summary>
	public List<AnswerRR> Answers;

	/// <summary>
	/// List of AuthorityRR records
	/// </summary>
	public List<AuthorityRR> Authorities;

	/// <summary>
	/// Error message, empty when no error
	/// </summary>
	public string Error;

	/// <summary>
	/// Header
	/// </summary>
	public Header header;

	/// <summary>
	/// The Size of the message
	/// </summary>
	public int MessageSize;

	/// <summary>
	/// List of Question records
	/// </summary>
	public List<Question> Questions;

	/// <summary>
	/// Server which delivered this response
	/// </summary>
	public IPEndPoint Server;

	/// <summary>
	/// TimeStamp when cached
	/// </summary>
	public DateTime TimeStamp;

	#endregion

	#region Constructors

	public Response()
	{
		MessageSize	= 0;
		Error		= string.Empty;
		header		= new Header();
		TimeStamp	= DateTime.Now;
		Answers		= new List<AnswerRR>();
		Authorities	= new List<AuthorityRR>();
		Additionals	= new List<AdditionalRR>();
		Questions	= new List<Question>();
		Server		= new IPEndPoint(0, 0);
	}

	public Response(IPEndPoint iPEndPoint, byte[] data)
	{
		Server			= iPEndPoint;
		Error			= string.Empty;
		MessageSize		= data.Length;
		TimeStamp		= DateTime.Now;
		Answers			= new List<AnswerRR>();
		Authorities		= new List<AuthorityRR>();
		Additionals		= new List<AdditionalRR>();
		Questions		= new List<Question>();
		RecordReader rr	= new RecordReader(data);
		header			= new Header(rr);

		//if (header.RCODE != RCode.NoError)
		//	Error = header.RCODE.ToString();

		for (int intI = 0; intI < header.QDCOUNT; intI++)
		{
			Questions.Add(new Question(rr));
		}

		for (int intI = 0; intI < header.ANCOUNT; intI++)
		{
			Answers.Add(new AnswerRR(rr));
		}

		for (int intI = 0; intI < header.NSCOUNT; intI++)
		{
			Authorities.Add(new AuthorityRR(rr));
		}
		for (int intI = 0; intI < header.ARCOUNT; intI++)
		{
			Additionals.Add(new AdditionalRR(rr));
		}
	}

	#endregion

	#region Fields

	/// <summary>
	/// List of RecordA in Response.Answers
	/// </summary>
	public RecordA[] RecordsA
	{
		get
		{
			List<RecordA> list = new List<RecordA>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordA record = answerRR.RECORD as RecordA;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordAAAA in Response.Answers
	/// </summary>
	public RecordAAAA[] RecordsAAAA
	{
		get
		{
			List<RecordAAAA> list = new List<RecordAAAA>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordAAAA record = answerRR.RECORD as RecordAAAA;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordCNAME in Response.Answers
	/// </summary>
	public RecordCNAME[] RecordsCNAME
	{
		get
		{
			List<RecordCNAME> list = new List<RecordCNAME>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordCNAME record = answerRR.RECORD as RecordCNAME;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordMX in Response.Answers
	/// </summary>
	public RecordMX[] RecordsMX
	{
		get
		{
			List<RecordMX> list = new List<RecordMX>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordMX record = answerRR.RECORD as RecordMX;
				if (record != null)
					list.Add(record);
			}
			list.Sort();
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordNS in Response.Answers
	/// </summary>
	public RecordNS[] RecordsNS
	{
		get
		{
			List<RecordNS> list = new List<RecordNS>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordNS record = answerRR.RECORD as RecordNS;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordPTR in Response.Answers
	/// </summary>
	public RecordPTR[] RecordsPTR
	{
		get
		{
			List<RecordPTR> list = new List<RecordPTR>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordPTR record = answerRR.RECORD as RecordPTR;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordsRR in Response.Additionals, Answers & Authorities
	/// </summary>
	public RR[] RecordsRR
	{
		get
		{
			List<RR> list = new List<RR>();
			foreach (RR rr in Answers)
			{
				list.Add(rr);
			}
			foreach (RR rr in Authorities)
			{
				list.Add(rr);
			}
			foreach (RR rr in Additionals)
			{
				list.Add(rr);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordSOA in Response.Answers
	/// </summary>
	public RecordSOA[] RecordsSOA
	{
		get
		{
			List<RecordSOA> list = new List<RecordSOA>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordSOA record = answerRR.RECORD as RecordSOA;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// List of RecordTXT in Response.Answers
	/// </summary>
	public RecordTXT[] RecordsTXT
	{
		get
		{
			List<RecordTXT> list = new List<RecordTXT>();
			foreach (AnswerRR answerRR in Answers)
			{
				RecordTXT record = answerRR.RECORD as RecordTXT;
				if (record != null)
					list.Add(record);
			}
			return list.ToArray();
		}
	}

	#endregion
}