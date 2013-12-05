using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Jaddie.Logging.Log4net
{
	public class LoggingProvider : ILoggingProvider
	{
		public LoggingProvider()
		{
			try
			{
				var customBehaviourEnabled = ConfigurationManager.AppSettings.Get("log4net.CustomBehaviourEnabled");
				if (customBehaviourEnabled != null && Convert.ToBoolean(customBehaviourEnabled))
				{
					var configPath = ConfigurationManager.AppSettings.Get("log4net.Config");
					if (!string.IsNullOrWhiteSpace(configPath))
					{
						var fileInfo = new FileInfo(configPath);
						XmlConfigurator.ConfigureAndWatch(fileInfo);
					}
				}
			}
			catch
			{
				// Ignore exceptions as if any occur they aren't important
			}
		}
		public void LogValue(SeverityLevel level, object value, object owner, Exception ex = null)
		{
			var type = owner as Type;
			var logger = type != null ? LogManager.GetLogger(type) : LogManager.GetLogger(Convert.ToString(owner));
			if (value is NameValueCollection)
			{
				var collection = (NameValueCollection)value;
				value = collection.Cast<string>().Aggregate<string, object>(string.Empty, (current, key) => (object)(current + ("Key: " + key + " Value: " + collection.Get(key))));
			}
			switch (level)
			{
				case SeverityLevel.Debug:
					logger.Logger.Log(typeof(LoggingProvider), Level.Debug, value, ex);
					break;
				case SeverityLevel.Info:
					logger.Logger.Log(typeof(LoggingProvider), Level.Info, value, ex);
					break;
				case SeverityLevel.Warn:
					logger.Logger.Log(typeof(LoggingProvider), Level.Warn, value, ex);
					break;
				case SeverityLevel.Error:
					logger.Logger.Log(typeof(LoggingProvider), Level.Error, value, ex);
					break;
				case SeverityLevel.Fatal:
					logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, value, ex);
					break;
			}
		}

		public void LogValue(SeverityLevel level, object owner, Exception ex = null, params object[] values)
		{
			var type = owner as Type;
			var logger = type != null ? LogManager.GetLogger(type) : LogManager.GetLogger(Convert.ToString(owner));
			foreach (var value in values.Where(value => value != null))
			{
				if (value is NameValueCollection)
				{
					var collection = (NameValueCollection)value;
					logger.Logger.Log(typeof(LoggingProvider), Level.Debug, collection.Cast<string>().Aggregate<string, object>(string.Empty, (current, key) => (object)(current + ("Key: " + key + " Value: " + collection.Get(key)))), null);
				}
				try //TODO: Work around try requirement
				{
					var logging = LogManager.GetLogger("PropertyNameLogger");
					var valuesType = value.GetType();
					foreach (var property in valuesType.GetProperties())
					{
						//logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property, null); // Note: This causes an extrem amount of logging.
						try //TODO: Work around try requirement
						{
							if (property.GetIndexParameters().Length > 0)
							{
								var count = -1;
								if (valuesType.GetProperty("Count") != null && valuesType.GetProperty("Count").PropertyType == typeof(Int32))
								{
									count = (int)valuesType.GetProperty("Count").GetValue(value, null);
								}
								if (count > 0)
								{
									for (var i = 0; i < count; i++)
									{
										try
										{
											var val = property.GetValue(value, new object[] { i });
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug,
												property + " " + val, null);
										}
										catch
										{

										}
									}
								}
							}
							else
							{
								try
								{
									var enumerable = property.GetValue(value, null) as IEnumerable<object>;
									if (enumerable != null)
									{
										foreach (var o in enumerable)
										{
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug, o, null);
										}
									}
								}
								catch (Exception e)
								{
									logging.Logger.Log(typeof(LoggingProvider), Level.Error, null, e);
								}
								logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property + " " + property.GetValue(value, null), null);
							}
						}
						catch (Exception e)
						{
							logger.Debug(e);
						}
					}
				}
				catch (Exception e)
				{
					logger.Error(e);
				}
				switch (level)
				{
					case SeverityLevel.Debug:
						logger.Logger.Log(typeof(LoggingProvider), Level.Debug, value, ex);
						break;
					case SeverityLevel.Info:
						logger.Logger.Log(typeof(LoggingProvider), Level.Info, value, ex);
						break;
					case SeverityLevel.Warn:
						logger.Logger.Log(typeof(LoggingProvider), Level.Warn, value, ex);
						break;
					case SeverityLevel.Error:
						logger.Logger.Log(typeof(LoggingProvider), Level.Error, value, ex);
						break;
					case SeverityLevel.Fatal:
						logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, value, ex);
						break;
				}
			}
		}

		public void LogValue(SeverityLevel level, object owner, params object[] values)
		{
			var type = owner as Type;
			var logger = type != null ? LogManager.GetLogger(type) : LogManager.GetLogger(Convert.ToString(owner));
			foreach (var value in values.Where(value => value != null))
			{
				if (value is NameValueCollection)
				{
					var collection = (NameValueCollection)value;
					logger.Logger.Log(typeof(LoggingProvider), Level.Debug, collection.Cast<string>().Aggregate<string, object>(string.Empty, (current, key) => (object)(current + ("Key: " + key + " Value: " + collection.Get(key)))), null);
				}
				try //TODO: Work around try requirement
				{
					var logging = LogManager.GetLogger("PropertyNameLogger");
					var valuesType = value.GetType();
					foreach (var property in valuesType.GetProperties())
					{
						//logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property, null); // Note: This causes an extrem amount of logging.
						try //TODO: Work around try requirement
						{
							if (property.GetIndexParameters().Length > 0)
							{
								var count = -1;
								if (valuesType.GetProperty("Count") != null && valuesType.GetProperty("Count").PropertyType == typeof(Int32))
								{
									count = (int)valuesType.GetProperty("Count").GetValue(value, null);
								}
								if (count > 0)
								{
									for (var i = 0; i < count; i++)
									{
										try
										{
											var val = property.GetValue(value, new object[] { i });
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug,
												property + " " + val, null);
										}
										catch
										{

										}
									}
								}
							}
							else
							{
								try
								{
									var enumerable = property.GetValue(value, null) as IEnumerable<object>;
									if (enumerable != null)
									{
										foreach (var o in enumerable)
										{
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug, o, null);
										}
									}
								}
								catch (Exception e)
								{
									logging.Logger.Log(typeof(LoggingProvider), Level.Error, null, e);
								}
								logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property + " " + property.GetValue(value, null), null);
							}
						}
						catch (Exception e)
						{
							logger.Debug(e);
						}
					}
				}
				catch (Exception e)
				{
					logger.Error(e);
				}
				switch (level)
				{
					case SeverityLevel.Debug:
						logger.Logger.Log(typeof(LoggingProvider), Level.Debug, value, null);
						break;
					case SeverityLevel.Info:
						logger.Logger.Log(typeof(LoggingProvider), Level.Info, value, null);
						break;
					case SeverityLevel.Warn:
						logger.Logger.Log(typeof(LoggingProvider), Level.Warn, value, null);
						break;
					case SeverityLevel.Error:
						logger.Logger.Log(typeof(LoggingProvider), Level.Error, value, null);
						break;
					case SeverityLevel.Fatal:
						logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, value, null);
						break;
				}
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Debug(object objectToLog, object owner = null)
		{
			var type = owner as Type;
			var logger = type == null ? LogManager.GetLogger(Convert.ToString(owner)) : LogManager.GetLogger(type);
			if (objectToLog == null)
			{
				return;
			}
			if (objectToLog is NameValueCollection)
			{
				var values = (NameValueCollection)objectToLog;
				foreach (var key in values.AllKeys)
				{
					logger.Logger.Log(typeof(LoggingProvider), Level.Debug, "Key: " + key + " Value: " + values.Get(key), null);
				}
			}
			else
			{
				try //TODO: Work around try requirement
				{
					var logging = LogManager.GetLogger("PropertyNameLogger");
					var valuesType = objectToLog.GetType();
					foreach (var property in valuesType.GetProperties())
					{
						logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property, null);
						try //TODO: Work around try requirement
						{
							if (property.GetIndexParameters().Length > 0)
							{
								var count = -1;
								if (valuesType.GetProperty("Count") != null && valuesType.GetProperty("Count").PropertyType == typeof(Int32))
								{
									count = (int)valuesType.GetProperty("Count").GetValue(objectToLog, null);
								}
								if (count > 0)
								{
									for (var i = 0; i < count; i++)
									{
										try
										{
											var val = property.GetValue(objectToLog, new object[] { i });
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug, val, null);
										}
										catch
										{

										}
									}
								}
							}
							else
							{
								try
								{
									var enumerable = property.GetValue(objectToLog, null) as IEnumerable<object>;
									if (enumerable != null)
									{
										foreach (var o in enumerable)
										{
											logging.Logger.Log(typeof(LoggingProvider), Level.Debug, o, null);
										}
									}
								}
								catch (Exception e)
								{
									//logging.Logger.Log(typeof(LoggingProvider), Level.Error,null, e);
								}
								logging.Logger.Log(typeof(LoggingProvider), Level.Debug, property.GetValue(objectToLog, null), null);
							}
						}
						catch (Exception e)
						{
							//logger.Debug(e);
						}
					}
				}
				catch (Exception e)
				{
					//logger.Debug(e);
				}
				logger.Logger.Log(typeof(LoggingProvider), Level.Debug, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Debug(object objectToLog, Type ownerType)
		{
			var logger = LogManager.GetLogger(ownerType);
			if (objectToLog is NameValueCollection)
			{
				var values = (NameValueCollection)objectToLog;
				foreach (var key in values.AllKeys)
				{
					logger.Logger.Log(typeof(LoggingProvider), Level.Debug, "Key: " + key + " Value: " + values.Get(key), null);
				}
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Debug, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Error(object objectToLog, object owner = null, Exception exception = null)
		{
			var logger = LogManager.GetLogger(Convert.ToString(owner));
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Error, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Error, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Error(object objectToLog, Type ownerType, Exception exception = null)
		{
			var logger = LogManager.GetLogger(ownerType);
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Error, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Error, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Fatal(object objectToLog, object owner = null, Exception exception = null)
		{
			var logger = LogManager.GetLogger(Convert.ToString(owner));
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Fatal(object objectToLog, Type ownerType, Exception exception = null)
		{
			var logger = LogManager.GetLogger(ownerType);
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Fatal, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Info(object objectToLog, object owner = null)
		{
			var logger = LogManager.GetLogger(Convert.ToString(owner));
			logger.Logger.Log(typeof(LoggingProvider), Level.Info, objectToLog, null);
		}

		[Obsolete("Use LogValue method instead")]
		public void Info(object objectToLog, Type ownerType = null)
		{
			var logger = LogManager.GetLogger(ownerType);
			logger.Logger.Log(typeof(LoggingProvider), Level.Info, objectToLog, null);
		}

		[Obsolete("Use LogValue method instead")]
		public void Warn(object objectToLog, object owner = null, Exception exception = null)
		{
			var logger = LogManager.GetLogger(Convert.ToString(owner));
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Warn, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Warn, objectToLog, null);
			}
		}

		[Obsolete("Use LogValue method instead")]
		public void Warn(object objectToLog, Type ownerType, Exception exception = null)
		{
			var logger = LogManager.GetLogger(ownerType);
			if (exception != null)
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Warn, objectToLog, exception);
			}
			else
			{
				logger.Logger.Log(typeof(LoggingProvider), Level.Warn, objectToLog, null);
			}
		}
	}
}