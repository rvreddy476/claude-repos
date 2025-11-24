# Google Cloud Storage Setup for Media Uploads

This document provides instructions for setting up Google Cloud Storage to handle media uploads (images, videos, audio) in the Chat System application.

## Prerequisites

1. A Google Cloud Platform (GCP) account
2. A GCP project created
3. Billing enabled on your GCP project

## Step 1: Install Required NuGet Package

Add the Google Cloud Storage NuGet package to the Infrastructure project:

```bash
cd backend/src/ChatSystem.Infrastructure
dotnet add package Google.Cloud.Storage.V1
```

## Step 2: Create a Google Cloud Storage Bucket

1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Navigate to **Cloud Storage** > **Buckets**
3. Click **CREATE BUCKET**
4. Configure your bucket:
   - **Name**: Choose a globally unique name (e.g., `your-app-media-bucket`)
   - **Location type**: Choose based on your needs (Multi-region, Dual-region, or Region)
   - **Storage class**: Standard (for frequently accessed data)
   - **Access control**: Fine-grained
   - **Protection tools**: Configure as needed (optional)
5. Click **CREATE**

## Step 3: Set Up Bucket Permissions

To make uploaded files publicly accessible:

1. Go to your bucket
2. Click on the **PERMISSIONS** tab
3. Click **ADD PRINCIPAL**
4. In the "New principals" field, enter: `allUsers`
5. In the "Select a role" dropdown, choose: **Cloud Storage** > **Storage Object Viewer**
6. Click **SAVE**
7. Confirm by clicking **ALLOW PUBLIC ACCESS**

## Step 4: Create a Service Account

1. Navigate to **IAM & Admin** > **Service Accounts**
2. Click **CREATE SERVICE ACCOUNT**
3. Fill in the details:
   - **Service account name**: e.g., `chat-system-storage`
   - **Service account description**: "Service account for Chat System media uploads"
4. Click **CREATE AND CONTINUE**
5. Grant the service account the following role:
   - **Storage Object Admin** (for full access to upload/delete files)
6. Click **CONTINUE** then **DONE**

## Step 5: Create and Download Service Account Key

1. Click on the service account you just created
2. Go to the **KEYS** tab
3. Click **ADD KEY** > **Create new key**
4. Choose **JSON** format
5. Click **CREATE**
6. The key file will be downloaded automatically
7. **IMPORTANT**: Store this file securely - it contains credentials that provide access to your GCS bucket

## Step 6: Configure secrets.json

Update your `backend/src/ChatSystem.API/secrets.json` file (create it if it doesn't exist):

```json
{
  "GoogleCloudStorageSettings": {
    "ProjectId": "your-gcp-project-id",
    "BucketName": "your-app-media-bucket",
    "CredentialsPath": "/path/to/your-service-account-key.json",
    "MediaCdnUrl": ""
  }
}
```

**Configuration Options:**

- `ProjectId`: Your GCP project ID
- `BucketName`: The name of your GCS bucket
- `CredentialsPath`: Absolute path to your service account JSON key file
- `MediaCdnUrl`: (Optional) If using Media CDN, enter the CDN URL here. Leave empty to use direct GCS URLs.

### Example for Development:

```json
{
  "GoogleCloudStorageSettings": {
    "ProjectId": "my-chat-app-12345",
    "BucketName": "my-chat-app-media",
    "CredentialsPath": "/Users/username/keys/chat-system-storage-key.json",
    "MediaCdnUrl": ""
  }
}
```

### Example for Production with Media CDN:

```json
{
  "GoogleCloudStorageSettings": {
    "ProjectId": "my-chat-app-production",
    "BucketName": "my-chat-app-media-prod",
    "CredentialsPath": "/app/secrets/gcs-key.json",
    "MediaCdnUrl": "https://media-cdn.example.com"
  }
}
```

## Step 7: (Optional) Set Up Google Media CDN

For better performance and global distribution:

1. Navigate to **Network Services** > **Media CDN**
2. Click **CREATE SERVICE**
3. Configure the CDN service:
   - **Origin**: Your GCS bucket
   - **Cache settings**: Configure based on your needs
4. Note the CDN URL and add it to your `MediaCdnUrl` configuration

## Step 8: Environment Variables (Alternative to secrets.json)

Instead of using secrets.json, you can set environment variables:

```bash
export GoogleCloudStorageSettings__ProjectId="your-project-id"
export GoogleCloudStorageSettings__BucketName="your-bucket-name"
export GoogleCloudStorageSettings__CredentialsPath="/path/to/key.json"
export GoogleCloudStorageSettings__MediaCdnUrl="https://your-cdn-url.com"
```

Or use Docker environment variables:

```yaml
environment:
  - GoogleCloudStorageSettings__ProjectId=your-project-id
  - GoogleCloudStorageSettings__BucketName=your-bucket-name
  - GoogleCloudStorageSettings__CredentialsPath=/app/secrets/gcs-key.json
  - GoogleCloudStorageSettings__MediaCdnUrl=
```

## Step 9: Test the Setup

1. Start your backend application
2. Open the frontend and try creating a post with an image or video
3. Check the backend logs for upload confirmation messages
4. Verify the file appears in your GCS bucket
5. Verify the file is accessible via the public URL

## API Endpoints

The following endpoints are available for media operations:

### Upload Single File
```
POST /api/media/upload?userId={userId}
Content-Type: multipart/form-data

Body: file (IFormFile)
```

### Upload Multiple Files
```
POST /api/media/upload-multiple?userId={userId}
Content-Type: multipart/form-data

Body: files[] (List<IFormFile>)
```

### Delete File
```
DELETE /api/media?fileUrl={encodedFileUrl}
```

## File Size and Type Restrictions

- **Maximum file size**: 100 MB per file
- **Maximum files per batch upload**: 10 files
- **Allowed image types**: JPEG, JPG, PNG, GIF, WebP
- **Allowed video types**: MP4, WebM, OGG, QuickTime
- **Allowed audio types**: MPEG, MP3, WAV, OGG, WebM

## File Organization

Files are automatically organized in the bucket using the following structure:

```
{userId}/
  ├── 2024/
  │   ├── 01/
  │   │   ├── 15/
  │   │   │   ├── {guid}.jpg
  │   │   │   └── {guid}.mp4
```

This organization:
- Prevents filename collisions using GUIDs
- Makes it easy to find files by user and date
- Helps with lifecycle management and cleanup

## Security Best Practices

1. **Never commit** your service account key file to version control
2. Add `*.json` (credential files) to `.gitignore`
3. Use IAM roles with minimal required permissions
4. Enable **Uniform bucket-level access** for better security
5. Set up **Object Lifecycle Management** to automatically delete old files
6. Consider enabling **Object Versioning** for recovery
7. Monitor access logs for unusual activity

## Troubleshooting

### Error: "Could not load file or assembly 'Google.Cloud.Storage.V1'"
- Run: `dotnet add package Google.Cloud.Storage.V1` in the Infrastructure project
- Run: `dotnet restore`

### Error: "The Application Default Credentials are not available"
- Verify `CredentialsPath` points to a valid JSON key file
- Verify the JSON key file has proper permissions (readable)

### Error: "Access Denied" when uploading
- Verify the service account has **Storage Object Admin** role
- Verify the bucket exists and is accessible

### Files not publicly accessible
- Check bucket permissions include `allUsers` with **Storage Object Viewer** role
- Verify CORS settings if accessing from web browsers

### CDN not serving files
- Verify the CDN is properly configured to use your GCS bucket as origin
- Check CDN cache settings and purge cache if needed
- Verify the MediaCdnUrl configuration is correct

## Cost Considerations

Google Cloud Storage pricing includes:
- **Storage costs**: Based on data stored per month
- **Network costs**: Egress (download) bandwidth
- **Operations costs**: API requests (uploads, deletes, etc.)

Media CDN adds additional costs but can reduce egress costs for frequently accessed files.

Monitor your usage in the [GCP Billing Dashboard](https://console.cloud.google.com/billing).

## Additional Resources

- [Google Cloud Storage Documentation](https://cloud.google.com/storage/docs)
- [Media CDN Documentation](https://cloud.google.com/media-cdn/docs)
- [Google.Cloud.Storage.V1 API Reference](https://googleapis.dev/dotnet/Google.Cloud.Storage.V1/latest/)
- [Best Practices for Cloud Storage](https://cloud.google.com/storage/docs/best-practices)
