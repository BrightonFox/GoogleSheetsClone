FROM ubuntu

SHELL ["/bin/bash", "-c"]

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

RUN mkdir /spreadsheet_server

RUN cd /spreadsheet_server && apt-get -y update && apt-get -y install \
  libboost-all-dev \
  git \
  build-essential \
  cmake \
  g++ \
  netcat \
  pkg-config \
  wget

  RUN cd /home && wget http://downloads.sourceforge.net/project/boost/boost/1.76.0/boost_1_76_0.tar.gz \
  && tar xfz boost_1_76_0.tar.gz \
  && rm boost_1_76_0.tar.gz \
  && cd boost_1_76_0 \
  && ./bootstrap.sh --prefix=/usr/local --with-libraries=program_options \
  && ./b2 install \
  && cd /home \
  && rm -rf boost_1_76_0

COPY . /spreadsheet_server

RUN cd /spreadsheet_server && g++ server.cpp -L /usr/lib/ -lboost_system -lboost_thread -lpthread

CMD ["spreadsheet_server/a.out"]
