# AWS ECS Deployment Setup

This repository contains GitHub Actions workflow to deploy both APIs (AwsTestApi and AWSTestApi2) to AWS ECS using Fargate.

## Prerequisites

### AWS Resources Setup

You can use either the provided Terraform configuration or manually create the resources.

#### Option 1: Using Terraform (Recommended)

1. Navigate to the terraform directory: `cd terraform`
2. Initialize Terraform: `terraform init`
3. Plan the deployment: `terraform plan`
4. Apply the configuration: `terraform apply`

This will create all necessary AWS resources including ECR repositories, ECS cluster, services, IAM roles, VPC, and security groups.

#### Option 2: Manual Setup

1. **ECR Repositories**: Create two ECR repositories in your AWS account:
   - `awstestapi`
   - `awstestapi2`

2. **ECS Cluster**: Create an ECS cluster named `apis-cluster`

3. **ECS Services**: Create two ECS services in the cluster:
   - `awstestapi-service` 
   - `awstestapi2-service`

4. **IAM Roles**: Ensure you have the following IAM roles:
   - `ecsTaskExecutionRole` - for ECS to pull images and write logs
   - `ecsTaskRole` - for your applications to access AWS resources

5. **CloudWatch Log Groups**: Create log groups:
   - `/ecs/awstestapi`
   - `/ecs/awstestapi2`

6. **VPC and Security Groups**: Set up VPC, subnets, and security groups to allow traffic on port 8080

### GitHub Secrets Setup

Add the following secrets to your GitHub repository:

- `AWS_ACCESS_KEY_ID`: Your AWS access key ID
- `AWS_SECRET_ACCESS_KEY`: Your AWS secret access key

### Configuration Updates

1. **AWS Region**: If you're not using `us-east-1`, update the region in:
   - `.github/workflows/deploy-apis.yml`
   - Task definition files
   - Terraform variables (if using Terraform)

2. **Repository Names**: If you want different ECR repository names, update them in:
   - `.github/workflows/deploy-apis.yml`
   - Terraform configuration

## How It Works

The workflow triggers on:
- Push to `main` branch with changes in `AwsTestApi/` or `AWSTestApi2/` directories
- Pull requests affecting the same directories

The workflow includes:
1. **Change Detection**: Only deploys APIs that have changes
2. **Docker Build**: Builds Docker images for changed APIs
3. **ECR Push**: Pushes images to Amazon ECR
4. **ECS Deploy**: Updates ECS services with new task definitions

## Deployment Process

1. Make changes to either API
2. Push to `main` branch or create a pull request
3. GitHub Actions will:
   - Detect which APIs changed
   - Build and push Docker images to ECR
   - Update ECS task definitions with the AWS account ID
   - Deploy to ECS using Fargate

## AWS CLI Commands for Manual Setup

```bash
# Create ECR repositories
aws ecr create-repository --repository-name awstestapi --region us-east-1
aws ecr create-repository --repository-name awstestapi2 --region us-east-1

# Create ECS cluster
aws ecs create-cluster --cluster-name apis-cluster --region us-east-1

# Create log groups
aws logs create-log-group --log-group-name /ecs/awstestapi --region us-east-1
aws logs create-log-group --log-group-name /ecs/awstestapi2 --region us-east-1
```

## Project Structure

```
??? .github/workflows/
?   ??? deploy-apis.yml          # GitHub Actions workflow
??? AwsTestApi/
?   ??? Dockerfile               # Docker configuration for AwsTestApi
?   ??? task-definition.json     # ECS task definition for AwsTestApi
?   ??? ...                      # API source code
??? AWSTestApi2/
?   ??? Dockerfile               # Docker configuration for AWSTestApi2
?   ??? task-definition.json     # ECS task definition for AWSTestApi2
?   ??? ...                      # API source code
??? terraform/
?   ??? main.tf                  # Terraform configuration for AWS resources
??? DEPLOYMENT_README.md         # This file
```

## Features

- **Smart Change Detection**: Only builds and deploys APIs that have actual changes
- **Automatic Account ID Resolution**: The workflow automatically detects your AWS account ID
- **Container Optimization**: Both APIs use .NET 9 runtime optimized Docker containers
- **Secure**: Uses IAM roles for secure AWS resource access
- **Scalable**: Uses Fargate for serverless container orchestration
- **Monitored**: Integrated CloudWatch logging

## Notes

- Both APIs are configured to run on port 8080 inside the container
- The workflow uses Fargate with 256 CPU units and 512 MB memory
- Logs are sent to CloudWatch Logs
- The deployment waits for service stability before completing
- Task definitions use placeholder `{{AWS_ACCOUNT_ID}}` which gets replaced during deployment
- The AwsTestApi includes AWS S3 integration and requires appropriate IAM permissions