using System;
using System.Collections.Generic;
using System.IO;
using DotNetHelper_Contracts.Enum.IO;

namespace DotNetHelper_IO.Interface
{
    public interface ISerializer   : IConvertible
    {
        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize"></param>
        /// <param name="leaveStreamOpen"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToStream<T>(T obj, Stream stream, int bufferSize = 1024, bool leaveStreamOpen = false) where T : class;


        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="type"></param>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize"></param>
        /// <param name="leaveStreamOpen"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToStream(object obj, Type type, Stream stream, int bufferSize = 1024, bool leaveStreamOpen = false);

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="fullFilePath"></param>
        /// <param name="mode"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToFile<T>(T obj, string fullFilePath, FileOption mode) where T : class;

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="mode"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToFile<T>(IEnumerable<T> list, string fullFilePath, FileOption mode) where T : class;

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="type"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="mode"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToFile(object obj, Type type, string fullFilePath, FileOption mode);

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="fullFilePath"></param>
        /// <param name="mode"></param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        void SerializeToFile(dynamic obj, string fullFilePath, FileOption mode);


        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        string SerializeToString(object obj);



        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        string SerializeToString<T>(T obj) where T : class;

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fullFilePath"></param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        object DeserializeFromFile(Type type, string fullFilePath);

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFilePath"></param>
        /// <returns>``0.</returns>
        /// <exception cref="System.ArgumentNullException">file</exception>
        T DeserializeFromFile<T>(string fullFilePath) where T : class;


        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFilePath"></param>
        /// <returns>``0.</returns>
        /// <exception cref="System.ArgumentNullException">file</exception>
        // List<T> DeserializeFromFilet<T>(string fullFilePath) where T : IEnumerable<T>;

        /// <summary>
        /// Deserializes from file to a dynamic Object
        /// </summary>
        /// <param name="fullFilePath">The full file path.</param>
        /// <returns>dynamic.</returns>
        dynamic DeserializeFromFile(string fullFilePath);

        /// <summary>
        /// Deserializes from file to a list of dynamic objects 
        /// </summary>
        /// <param name="fullFilePath">The full file path.</param>
        /// <returns>IEnumerable&lt;dynamic&gt;.</returns>
        IEnumerable<dynamic> DeserializeListFromFile(string fullFilePath);

        /// <summary>
        /// Deserializes from a string to a list of dynamic objects 
        /// </summary>
        /// <param name="content">A delimited CSV string.</param>
        /// <returns>IEnumerable&lt;dynamic&gt;.</returns>
        IEnumerable<dynamic> DeserializeToList(string content);

        /// <summary>
        /// Deserializes from file to a list of dynamic objects 
        /// </summary>
        /// <param name="fullFilePath">The full file path.</param>
        /// <returns>IEnumerable&lt;dynamic&gt;.</returns>
        IEnumerable<T> DeserializeListFromFile<T>(string fullFilePath) where T : class;

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>``0.</returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        T DeserializeFromStream<T>(Stream stream);

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        object DeserializeFromStream(Stream stream, Type type);

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns>``0.</returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        T DeserializeFromString<T>(string content) where T : class;

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns>List Of T</returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        List<T> DeserializeToList<T>(string content) where T : class;

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        object DeserializeFromString(string content, Type type);


        string DeserializeToCSharpClass(string content, string className = null);

    }
}
