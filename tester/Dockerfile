FROM ubuntu

SHELL ["/bin/bash", "-c"]

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

RUN apt-get -y update && apt-get -y install \
libboost-all-dev \
git \
build-essential \
cmake \
g++ \
pkg-config \
software-properties-common \
wget


RUN cd /home && wget http://downloads.sourceforge.net/project/boost/boost/1.76.0/boost_1_76_0.tar.gz \
&& tar xfz boost_1_76_0.tar.gz \
&& rm boost_1_76_0.tar.gz \
&& cd boost_1_76_0 \
&& ./bootstrap.sh --prefix=/usr/local --with-libraries=program_options \
&& ./b2 install \
&& cd /home \
&& rm -rf boost_1_76_0


COPY . ./

RUN make
