﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Junior.Common;
using Junior.Route.Routing.RequestValueComparers;

namespace Junior.Route.Routing.Restrictions
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class HeaderRestriction : IRestriction, IEquatable<HeaderRestriction>
	{
		private readonly string _field;
		private readonly bool _optional;
		private readonly string _value;
		private readonly IRequestValueComparer _valueComparer;

		public HeaderRestriction(string field, string value, IRequestValueComparer valueComparer, bool optional = false)
		{
			field.ThrowIfNull("header");
			value.ThrowIfNull("value");
			valueComparer.ThrowIfNull("valueComparer");

			_field = field;
			_value = value;
			_valueComparer = valueComparer;
			_optional = optional;
		}

		public string Field
		{
			get
			{
				return _field;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}

		public IRequestValueComparer ValueComparer
		{
			get
			{
				return _valueComparer;
			}
		}

		public bool Optional
		{
			get
			{
				return _optional;
			}
		}

		// ReSharper disable UnusedMember.Local
		private string DebuggerDisplay
			// ReSharper restore UnusedMember.Local
		{
			get
			{
				return String.Format("{0}: {1}", _field, _value);
			}
		}

		public bool Equals(HeaderRestriction other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return String.Equals(_field, other._field) && String.Equals(_value, other._value) && Equals(_valueComparer, other._valueComparer);
		}

		public MatchResult MatchesRequest(HttpRequestBase request)
		{
			request.ThrowIfNull("request");

			IEnumerable<string> matchingKeys = request.Headers.AllKeys.Where(header => String.Equals(_field, header, StringComparison.OrdinalIgnoreCase)).ToArray();

			return (_optional && !matchingKeys.Any()) || matchingKeys.Any(arg => _valueComparer.Matches(_value, request.Headers[arg]))
				? MatchResult.RestrictionMatched(this.ToEnumerable())
				: MatchResult.RestrictionNotMatched(Enumerable.Empty<IRestriction>(), this.ToEnumerable());
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
			return Equals((HeaderRestriction)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (_field != null ? _field.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ (_value != null ? _value.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (_valueComparer != null ? _valueComparer.GetHashCode() : 0);

				return hashCode;
			}
		}
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class HeaderRestriction<T> : IRestriction
	{
		private readonly string _field;
		private readonly Func<T, bool> _matchDelegate;
		private readonly bool _optional;
		private readonly Func<string, IEnumerable<T>> _parseDelegate;

		public HeaderRestriction(string field, Func<string, IEnumerable<T>> parseDelegate, Func<T, bool> matchDelegate, bool optional = false)
		{
			field.ThrowIfNull("header");
			parseDelegate.ThrowIfNull("parseDelegate");
			matchDelegate.ThrowIfNull("matchDelegate");

			_field = field;
			_parseDelegate = parseDelegate;
			_matchDelegate = matchDelegate;
			_optional = optional;
		}

		public HeaderRestriction(string field, Func<string, T> parseDelegate, Func<T, bool> matchDelegate, bool optional = false)
			: this(field, headerValue => parseDelegate(headerValue).ToEnumerable(), matchDelegate, optional)
		{
		}

		public string Field
		{
			get
			{
				return _field;
			}
		}

		public bool Optional
		{
			get
			{
				return _optional;
			}
		}

		// ReSharper disable UnusedMember.Local
		private string DebuggerDisplay
			// ReSharper restore UnusedMember.Local
		{
			get
			{
				return _field;
			}
		}

		public MatchResult MatchesRequest(HttpRequestBase request)
		{
			request.ThrowIfNull("request");

			IEnumerable<string> matchingKeys = request.Headers.AllKeys.Where(header => String.Equals(_field, header, StringComparison.OrdinalIgnoreCase)).ToArray();

			return (_optional && !matchingKeys.Any()) || _parseDelegate(request.Headers[_field]).Any(_matchDelegate)
				? MatchResult.RestrictionMatched(this.ToEnumerable())
				: MatchResult.RestrictionNotMatched(Enumerable.Empty<IRestriction>(), this.ToEnumerable());
		}
	}
}