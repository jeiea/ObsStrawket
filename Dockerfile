FROM ghcr.io/catthehacker/ubuntu:custom-latest-20230829

RUN apt update
RUN apt install fd-find fzf less neovim
RUN ln -s $(which fdfind) /usr/local/bin/fd