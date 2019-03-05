using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace CSVToESLib.Template
{
    internal class ElasticsearchClient
    {
        protected const int BufferSize = 81920;
        private const string newLineString = "\n";

        public PostType Type { get; protected set; }
        public byte[] WrittenBytes { get; protected set; }

        private IEnumerable<string> _enumurableOfStrings = null;
        ElasticLowLevelClient ElasticLowLevelClient { get; set; }
        public ElasticsearchClient(ConnectionConfiguration configuration)
        {
            ElasticLowLevelClient = new ElasticLowLevelClient(configuration);
        }

        public async Task<StringResponse> BulkInsert(ParallelQuery<CsvMappingResult<Person>> results, int version)
        {
            var test = results.Where(result => result.IsValid).Select(result => { result.Result.Version = version; return new Index(result.Result).ToString(); });
            PostData postData = PostData.MultiJson(test.ToList());
            //var isTestAny = test.Any();
            //File.WriteAllLines("anyTest.txt", test, System.Text.Encoding.UTF8);
            postData.Write(System.IO.File.OpenWrite("Output2.txt"), new ConnectionConfiguration(uri: null));
            ////System.IO.File.WriteAllLines("output.txt", postData., System.Text.Encoding.UTF8);

            return await ElasticLowLevelClient.BulkAsync<StringResponse>(Elasticsearch.Net.PostData.MultiJson(test));
        }

    }

    public class Index
    {
        public InnerIndex InnerIndex;
        public Person Person;

        public Index(Person person)
        {
            Person = person;
            InnerIndex = new InnerIndex();
        }

        public override string ToString()
        {
            return InnerIndex.ToString() + "\n" + Person.ToString();
        }
    }

    public class InnerIndex
    {
        public string _index;
        public string _type;

        public InnerIndex(string index = null, string type = null)
        {
            _index = index;
            _type = type;
        }

        public override string ToString()
        {
            return "{\"index\":{\"_index\":\"persons\",\"_type\":\"person\"}}";
        }
    }

    public interface IPostData
    {
        void Write(Stream writableStream, IConnectionConfigurationValues settings);
    }

    public interface IPostData<out T> : IPostData { }

    public enum PostType
    {
        ByteArray,
        LiteralString,
        EnumerableOfString,
        EnumerableOfObject,
        Serializable
    }

    public abstract class PostData
    {
        protected const int BufferSize = 81920;
        protected const string NewLineString = "\n";
        protected static readonly byte[] NewLineByteArray = { (byte)'\n' };

        public bool? DisableDirectStreaming { get; set; }
        public PostType Type { get; protected set; }
        public byte[] WrittenBytes { get; protected set; }

        public abstract void Write(Stream writableStream, IConnectionConfigurationValues settings);

        public abstract Task WriteAsync(Stream writableStream, IConnectionConfigurationValues settings, CancellationToken cancellationToken);

        public static implicit operator PostData(byte[] byteArray) => Bytes(byteArray);

        public static implicit operator PostData(string literalString) => String(literalString);

        public static SerializableData<T> Serializable<T>(T o) => new SerializableData<T>(o);

        public static PostData MultiJson(IEnumerable<string> listOfString) => new PostData<object>(listOfString);

        public static PostData MultiJson(IEnumerable<object> listOfObjects) => new PostData<object>(listOfObjects);

        public static PostData Bytes(byte[] bytes) => new PostData<object>(bytes);

        public static PostData String(string serializedString) => new PostData<object>(serializedString);
    }

    public class PostData<T> : PostData, IPostData<T>
    {
        private readonly IEnumerable<object> _enumerableOfObject;
        private readonly IEnumerable<string> _enumurableOfStrings;
        private readonly string _literalString;
        private readonly T _serializable;

        protected internal PostData(byte[] item)
        {
            WrittenBytes = item;
            Type = PostType.ByteArray;
        }

        protected internal PostData(string item)
        {
            _literalString = item;
            Type = PostType.LiteralString;
        }

        protected internal PostData(IEnumerable<string> item)
        {
            _enumurableOfStrings = item;
            Type = PostType.EnumerableOfString;
        }

        protected internal PostData(IEnumerable<object> item)
        {
            _enumerableOfObject = item;
            Type = PostType.EnumerableOfObject;
        }

        private PostData(T item)
        {
            var boxedType = item.GetType();
            if (typeof(byte[]).AssignableFrom(boxedType))
            {
                WrittenBytes = item as byte[];
                Type = PostType.ByteArray;
            }
            else if (typeof(string).AssignableFrom(boxedType))
            {
                _literalString = item as string;
                Type = PostType.LiteralString;
            }
            else if (typeof(IEnumerable<string>).AssignableFrom(boxedType))
            {
                _enumurableOfStrings = (IEnumerable<string>)item;
                Type = PostType.EnumerableOfString;
            }
            else if (typeof(IEnumerable<object>).AssignableFrom(boxedType))
            {
                _enumerableOfObject = (IEnumerable<object>)item;
                Type = PostType.EnumerableOfObject;
            }
            else
            {
                _serializable = item;
                Type = PostType.Serializable;
            }
        }

        public override void Write(Stream writableStream, IConnectionConfigurationValues settings)
        {
            string UseStringBuilderConcat(IEnumerable<string> vs)
            {
                var stringBuilder = new StringBuilder();
                foreach (var item in vs)
                {
                    stringBuilder.Append(item + "\n");
                }
                return stringBuilder.ToString();
            }

            var indent = settings.PrettyJson ? SerializationFormatting.Indented : SerializationFormatting.None;
            MemoryStream ms = null;
            Stream stream = null;
            switch (Type)
            {
                case PostType.ByteArray:
                    ms = new MemoryStream(WrittenBytes);
                    break;
                case PostType.LiteralString:
                    ms = !string.IsNullOrEmpty(_literalString) ? new MemoryStream(_literalString?.Utf8Bytes()) : null;
                    break;
                case PostType.EnumerableOfString:
                    ms = _enumurableOfStrings.HasAny()
                        ? new MemoryStream(UseStringBuilderConcat(_enumurableOfStrings).Utf8Bytes())
                        : null;
                    break;
                case PostType.EnumerableOfObject:
                    if (!_enumerableOfObject.HasAny()) return;

                    if (DisableDirectStreaming ?? settings.DisableDirectStreaming)
                    {
                        ms = new MemoryStream();
                        stream = ms;
                    }
                    else stream = writableStream;
                    foreach (var o in _enumerableOfObject)
                    {
                        settings.RequestResponseSerializer.Serialize(o, stream, SerializationFormatting.None);
                        stream.Write(NewLineByteArray, 0, 1);
                    }
                    break;
                case PostType.Serializable:
                    stream = writableStream;
                    if (DisableDirectStreaming ?? settings.DisableDirectStreaming)
                    {
                        ms = new MemoryStream();
                        stream = ms;
                    }
                    settings.RequestResponseSerializer.Serialize(_serializable, stream, indent);
                    break;
            }
            if (ms != null)
            {
                ms.Position = 0;
                ms.CopyTo(writableStream, BufferSize);
            }
            if (Type != 0)
                WrittenBytes = ms?.ToArray();
        }

        public override async Task WriteAsync(Stream writableStream, IConnectionConfigurationValues settings, CancellationToken cancellationToken)
        {
            

            var indent = settings.PrettyJson ? SerializationFormatting.Indented : SerializationFormatting.None;
            MemoryStream ms = null;
            Stream stream = null;
            switch (Type)
            {
                case PostType.ByteArray:
                    ms = new MemoryStream(WrittenBytes);
                    break;
                case PostType.LiteralString:
                    ms = !string.IsNullOrEmpty(_literalString) ? new MemoryStream(_literalString.Utf8Bytes()) : null;
                    break;
                case PostType.EnumerableOfString:
                    ms = _enumurableOfStrings.HasAny()
                        ? new MemoryStream("Test".Utf8Bytes())
                        : null;
                    break;
                case PostType.EnumerableOfObject:
                    if (!_enumerableOfObject.HasAny()) return;

                    if (DisableDirectStreaming ?? settings.DisableDirectStreaming)
                    {
                        ms = new MemoryStream();
                        stream = ms;
                    }
                    else stream = writableStream;
                    foreach (var o in _enumerableOfObject)
                    {
                        await settings.RequestResponseSerializer.SerializeAsync(o, stream, SerializationFormatting.None, cancellationToken)
                            .ConfigureAwait(false);
                        await stream.WriteAsync(NewLineByteArray, 0, 1, cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case PostType.Serializable:
                    stream = writableStream;
                    if (DisableDirectStreaming ?? settings.DisableDirectStreaming)
                    {
                        ms = new MemoryStream();
                        stream = ms;
                    }
                    await settings.RequestResponseSerializer.SerializeAsync(_serializable, stream, indent, cancellationToken).ConfigureAwait(false);
                    break;
            }
            if (ms != null)
            {
                ms.Position = 0;
                await ms.CopyToAsync(writableStream, BufferSize, cancellationToken).ConfigureAwait(false);
            }
            if (Type != 0)
                WrittenBytes = ms?.ToArray();
        }
    }
}
