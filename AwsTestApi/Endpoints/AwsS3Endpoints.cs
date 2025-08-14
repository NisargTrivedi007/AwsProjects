using Amazon.S3;

namespace AwsTestApi.Endpoints
{
    public static class AwsS3Endpoints 
    {
        public static void MapS3Endpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("s3")
                .WithTags("S3");

            group.MapGet("buckets", async (IAmazonS3 s3Client) =>
            {
                var response = await s3Client.ListBucketsAsync();
                //var resp = response.Buckets.FirstOrDefault().
                return Results.Ok(response.Buckets);
            });
            group.MapPost("createBucket", async (IAmazonS3 s3Client, string bucketName) =>
            {
                var request = new Amazon.S3.Model.PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };
                var response = await s3Client.PutBucketAsync(request);
                return Results.Ok(response);
            });

            group.MapGet("bucket/{bucketName}", async (IAmazonS3 s3Client, string bucketName) =>
            {
                var request = new Amazon.S3.Model.ListObjectsV2Request
                {
                    BucketName = bucketName
                };
                var response = await s3Client.ListObjectsV2Async(request);
                return Results.Ok(response.S3Objects);
            });

            group.MapDelete("bucket/{bucketName}", async (IAmazonS3 s3Client, string bucketName, CancellationToken cancellationToken) =>
            {
                var request = new Amazon.S3.Model.DeleteBucketRequest
                {
                    BucketName = bucketName
                };
                await s3Client.DeleteBucketAsync(request, cancellationToken);
                return Results.Accepted($"Bucket {bucketName} deleted successfully.");
            });
        }
    }
}
