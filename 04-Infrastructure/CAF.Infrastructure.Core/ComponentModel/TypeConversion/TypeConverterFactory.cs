using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;


namespace CAF.Infrastructure.Core.ComponentModel
{
	public static class TypeConverterFactory
	{
		private static readonly ConcurrentDictionary<Type, ITypeConverter> _typeConverters = new ConcurrentDictionary<Type, ITypeConverter>();

		static TypeConverterFactory()
		{
			CreateDefaultConverters();
		}

		private static void CreateDefaultConverters()
		{
			_typeConverters.TryAdd(typeof(DateTime), new DateTimeConverter());
			_typeConverters.TryAdd(typeof(TimeSpan), new TimeSpanConverter());
			_typeConverters.TryAdd(typeof(bool), new BooleanConverter(
				new [] { "yes", "y", "on", "wahr" },
				new [] { "no", "n", "off", "falsch" }));

		
		}

		public static void RegisterConverter<T>(ITypeConverter typeConverter)
		{
			RegisterConverter(typeof(T), typeConverter);
		}

		public static void RegisterConverter(Type type, ITypeConverter typeConverter)
		{
			Guard.ArgumentNotNull(() => type);
			Guard.ArgumentNotNull(() => typeConverter);

			_typeConverters.TryAdd(type, typeConverter);
        }

		public static ITypeConverter RemoveConverter<T>(ITypeConverter typeConverter)
		{
			return RemoveConverter(typeof(T));
		}

		public static ITypeConverter RemoveConverter(Type type)
		{
			Guard.ArgumentNotNull(() => type);

			ITypeConverter converter = null;
			_typeConverters.TryRemove(type, out converter);
			return converter;
		}

		public static ITypeConverter GetConverter<T>()
		{
			return GetConverter(typeof(T));
		}

		public static ITypeConverter GetConverter(object component)
		{
			Guard.ArgumentNotNull(() => component);

			return GetConverter(component.GetType());
		}

		public static ITypeConverter GetConverter(Type type)
		{
			Guard.ArgumentNotNull(() => type);

			ITypeConverter converter;
			if (_typeConverters.TryGetValue(type, out converter))
			{
				return converter;
			}
			
			var isGenericType = type.IsGenericType;
			if (isGenericType)
			{
				var definition = type.GetGenericTypeDefinition();

				// Nullables
				if (definition == typeof(Nullable<>))
				{
					converter = new NullableConverter(type);
					RegisterConverter(type, converter);
					return converter;
				}

				// Sequence types
				var genericArgs = type.GetGenericArguments();
				var isEnumerable = genericArgs.Length == 1 && type.IsSubClass(typeof(IEnumerable<>));
				if (isEnumerable)
				{
					converter = (ITypeConverter)Activator.CreateInstance(typeof(EnumerableConverter<>).MakeGenericType(genericArgs[0]), type);
					RegisterConverter(type, converter);
					return converter;
				}
			}

			// default fallback
			converter = new TypeConverterAdapter(TypeDescriptor.GetConverter(type));
			RegisterConverter(type, converter);
			return converter;
		}
	}
}
