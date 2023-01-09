echo "1/3 CLEAN UP..."
docker stop  binancewebsockettask_webapi && docker rm binancewebsockettask_webapi && docker rmi binancewebsockettask_webapi_image

echo "2/3 BUILD..."
docker build -f DockerfileWebApi . -t binancewebsockettask_webapi_image

echo "3/3 RUN..."
docker run -d --name binancewebsockettask_webapi -p 5000:80 binancewebsockettask_webapi_image