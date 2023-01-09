echo "1/3 CLEAN UP..."
docker stop binancewebsockettask_console_app && docker rm binancewebsockettask_console_app && docker rmi binancewebsockettask_console_app_image

echo "2/3 BUILD..."
docker build -f DockerfileConsoleApp . -t binancewebsockettask_console_app_image

echo "3/3 RUN..."
docker run --name binancewebsockettask_console_app binancewebsockettask_console_app_image