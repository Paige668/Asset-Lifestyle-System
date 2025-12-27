# AWS EC2 Deployment Guide

This guide walks you through deploying the IT Asset Manager application on AWS EC2 using Docker.

## Prerequisites

- AWS Account
- Basic knowledge of AWS Console
- SSH client installed on your local machine

## Estimated Costs

- **EC2 Instance (t3.small)**: ~$15/month
- **Storage (20 GB)**: ~$2/month
- **Data Transfer**: Minimal for development/demo

**Total**: ~$17-20/month

---

## Deployment Steps

### Step 1: Create EC2 Instance

1. **Login to AWS Console**: Navigate to EC2 Dashboard
2. **Launch Instance**:
   - Click "Launch Instance"
   - **Name**: `ITAssetManager-Demo`
   - **AMI**: Ubuntu Server 22.04 LTS (Free tier eligible)
   - **Instance Type**: `t3.small` (2 vCPU, 2 GB RAM) - Minimum recommended
   - **Key Pair**: Create new or select existing (save `.pem` file securely)
   - **Storage**: 20 GB gp3

3. **Configure Security Group**:

   Create a new security group with the following **Inbound Rules**:

   | Type        | Protocol | Port Range | Source    | Description     |
   |-------------|----------|------------|-----------|-----------------|
   | SSH         | TCP      | 22         | Your IP   | SSH access      |
   | HTTP        | TCP      | 80         | 0.0.0.0/0 | Frontend        |
   | Custom TCP  | TCP      | 4200       | 0.0.0.0/0 | Angular frontend |
   | Custom TCP  | TCP      | 8080       | 0.0.0.0/0 | API & Swagger   |

   > [!WARNING]
   > For production, restrict `0.0.0.0/0` to specific IP ranges and enable HTTPS with SSL certificates.

4. **Launch Instance** and wait for status to become "Running"

---

### Step 2: Connect to EC2 Instance

```bash
# Change permissions on your key pair
chmod 400 your-key-pair.pem

# Connect via SSH (replace with your instance's public IP)
ssh -i "your-key-pair.pem" ubuntu@<EC2-PUBLIC-IP>
```

---

### Step 3: Upload Project Files

**Option A: Using SCP (from your local machine)**:

```bash
# Navigate to your project directory locally
cd /path/to/ITAssetManager

# Upload entire project to EC2
scp -i "your-key-pair.pem" -r . ubuntu@<EC2-PUBLIC-IP>:/home/ubuntu/ITAssetManager
```

**Option B: Using Git (recommended)**:

```bash
# On EC2 instance
cd /home/ubuntu
git clone <your-repository-url> ITAssetManager
cd ITAssetManager
```

---

### Step 4: Run Deployment Script

```bash
# Navigate to project directory
cd /home/ubuntu/ITAssetManager

# Make script executable (if not already)
chmod +x deployment/deploy-ec2.sh

# Run deployment
./deployment/deploy-ec2.sh
```

The script will:

- ✅ Update system packages
- ✅ Install Docker & Docker Compose
- ✅ Build and start containers
- ✅ Display access URLs

**Expected Output**:

```
✅ Deployment Complete!
🌐 Access your application:
  - Frontend: http://<PUBLIC-IP>:4200
  - Backend API: http://<PUBLIC-IP>:8080
  - Swagger UI: http://<PUBLIC-IP>:8080/swagger
```

---

### Step 5: Verify Deployment

1. **Check Container Status**:

   ```bash
   sudo docker-compose ps
   ```

   Both `backend` and `sqlserver` should be "Up"

2. **View Logs**:

   ```bash
   sudo docker-compose logs -f backend
   ```

3. **Test API**:
   - Open browser: `http://<EC2-PUBLIC-IP>:8080/swagger`
   - Execute `GET /api/Assets` - should return empty array `[]`
   - Execute `POST /api/Assets` to create test asset

4. **Test Frontend**:
   - Open browser: `http://<EC2-PUBLIC-IP>:4200`
   - Should see the Angular application

---

## Environment Configuration

### Production Settings

Edit `docker-compose.yml` for production:

```yaml
services:
  backend:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;...
    restart: unless-stopped

  sqlserver:
    environment:
      - SA_PASSWORD=${SQL_SA_PASSWORD}  # Use secrets management
    restart: unless-stopped
```

### SSL/HTTPS Setup (Recommended for Production)

Use AWS Certificate Manager + Application Load Balancer:

1. Create SSL certificate in ACM
2. Set up Application Load Balancer
3. Configure target groups for ports 8080 and 4200
4. Update security groups to allow ALB traffic only

---

## Maintenance Commands

```bash
# View all logs
sudo docker-compose logs -f

# Restart services
sudo docker-compose restart

# Stop all services
sudo docker-compose down

# Rebuild and restart
sudo docker-compose up -d --build

# Check disk usage
df -h

# Clean up Docker resources
sudo docker system prune -a
```

---

## Troubleshooting

### Issue: Containers won't start

```bash
# Check logs for errors
sudo docker-compose logs backend
sudo docker-compose logs sqlserver

# Common fix: Database initialization delay
sudo docker-compose restart backend
```

### Issue: Cannot connect to API

- Verify security group rules allow inbound traffic on port 8080
- Check if backend container is running: `sudo docker-compose ps`
- Test from EC2 instance: `curl http://localhost:8080/swagger`

### Issue: Database connection errors

- SQL Server may need more time to initialize (wait 30-60 seconds)
- Check password meets complexity requirements in `docker-compose.yml`

---

## Backup & Data Persistence

Database data is stored in Docker volume. To backup:

```bash
# Backup database
sudo docker exec itassetmanager-sqlserver-1 /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123' \
  -Q "BACKUP DATABASE ITAssetDB TO DISK='/var/opt/mssql/backup/ITAssetDB.bak'"

# Copy backup to host
sudo docker cp itassetmanager-sqlserver-1:/var/opt/mssql/backup/ITAssetDB.bak ./backup/
```

---

## Cost Optimization

1. **Use t3.micro** for testing (Free tier eligible, but slower)
2. **Stop instance** when not in use (EC2 Dashboard → Instance State → Stop)
3. **Use Reserved Instances** for long-term deployments (up to 72% savings)
4. **Set up billing alerts** in AWS Billing Console

---

## Next Steps

- [ ] Configure custom domain name
- [ ] Set up HTTPS with Let's Encrypt or AWS Certificate Manager
- [ ] Implement CI/CD pipeline with GitHub Actions
- [ ] Add monitoring with CloudWatch
- [ ] Set up automated backups
- [ ] Configure Auto Scaling (for production)

---

## Security Checklist

- ✅ Changed default SQL Server password
- ✅ Restricted SSH access to your IP only
- ✅ Enabled AWS CloudWatch logging
- ✅ Configured firewall rules properly
- ⚠️ TODO: Enable HTTPS
- ⚠️ TODO: Implement authentication/authorization
- ⚠️ TODO: Set up VPC with private subnets for database

---

## Support

For issues or questions:

- Check application logs: `sudo docker-compose logs`
- Verify AWS security group settings
- Review this guide's troubleshooting section
