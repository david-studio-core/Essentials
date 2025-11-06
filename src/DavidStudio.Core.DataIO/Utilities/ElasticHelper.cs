using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace DavidStudio.Core.DataIO.Utilities;

public static class ElasticHelper
{
    public static Query EitherShouldHavePropertyWithValueOrNot(string field, string value)
    {
        return new BoolQuery
        {
            Should =
            [
                new TermQuery
                {
                    Field = field,
                    Value = value
                },
                new BoolQuery
                {
                    MustNot =
                    [
                        new ExistsQuery
                        {
                            Field = new Field(field)
                        }
                    ]
                }
            ],
            MinimumShouldMatch = 1
        };
    }

    public static Query[] EitherShouldHavePropertyInRangeOrNot(string field, double? from, double? to)
    {
        List<Query> queries = [];

        if (from is not null)
        {
            queries.Add(new NumberRangeQuery(new Field(field)) { Gte = from });
        }

        if (to is not null)
        {
            queries.Add(new NumberRangeQuery(new Field(field)) { Lte = to });
        }

        return queries.ToArray();
    }

    public static Query[] EitherShouldHavePropertyInRangeOrNot(string field, DateTime? from, DateTime? to)
    {
        List<Query> queries = [];

        if (from is not null)
        {
            queries.Add(new DateRangeQuery(new Field(field)) { Gte = from });
        }

        if (to is not null)
        {
            queries.Add(new DateRangeQuery(new Field(field)) { Lte = to });
        }

        return queries.ToArray();
    }

    public static Query ShouldBeInBoundingBox(
        string field,
        double topLeftLat, double topLeftLon,
        double bottomRightLat, double bottomRightLon)
    {
        return new GeoBoundingBoxQuery
        {
            Field = new Field(field),
            BoundingBox = GeoBounds.TopLeftBottomRight(new TopLeftBottomRightGeoBounds
            {
                TopLeft = GeoLocation.LatitudeLongitude(new LatLonGeoLocation { Lat = topLeftLat, Lon = topLeftLon }),
                BottomRight = GeoLocation.LatitudeLongitude(new LatLonGeoLocation { Lat = bottomRightLat, Lon = bottomRightLon })
            })
        };
    }

    public static SortOptions NearestToFurthestSort(string field, double lat, double lon)
    {
        return new SortOptions
        {
            GeoDistance = new GeoDistanceSort
            {
                Field = new Field(field),
                Location =
                [
                    GeoLocation.LatitudeLongitude(new LatLonGeoLocation
                    {
                        Lat = lat,
                        Lon = lon
                    })
                ],
                Order = SortOrder.Asc,
                Unit = DistanceUnit.Meters,
                Mode = SortMode.Min,
                DistanceType = GeoDistanceType.Arc,
                IgnoreUnmapped = false
            }
        };
    }

    public static PointInTimeReference CreatePitReference(string pit, string keepAlive)
    {
        return new PointInTimeReference
        {
            Id = pit,
            KeepAlive = keepAlive
        };
    }
}