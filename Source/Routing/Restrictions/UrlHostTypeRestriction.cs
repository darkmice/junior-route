﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Junior.Common;

namespace Junior.Route.Routing.Restrictions
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class UrlHostTypeRestriction : IRestriction, IEquatable<UrlHostTypeRestriction>
	{
		private readonly UriHostNameType _type;

		public UrlHostTypeRestriction(UriHostNameType type)
		{
			_type = type;
		}

		public UriHostNameType Type
		{
			get
			{
				return _type;
			}
		}

		// ReSharper disable UnusedMember.Local
		private string DebuggerDisplay
			// ReSharper restore UnusedMember.Local
		{
			get
			{
				return _type.ToString();
			}
		}

		public bool Equals(UrlHostTypeRestriction other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return _type.Equals(other._type);
		}

		public MatchResult MatchesRequest(HttpRequestBase request)
		{
			request.ThrowIfNull("request");

			return _type == request.Url.HostNameType ? MatchResult.RestrictionMatched(this.ToEnumerable()) : MatchResult.RestrictionNotMatched(Enumerable.Empty<IRestriction>(), this.ToEnumerable());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((UrlHostTypeRestriction)obj);
		}

		public override int GetHashCode()
		{
			return _type.GetHashCode();
		}
	}
}