#ifndef HEADER_KRPC_SERVICE
#define HEADER_KRPC_SERVICE

#include "krpc/client.hpp"

namespace krpc {

  class Service {
  protected:
    Client* _client;
  public:
    Service(Client* client);
  };

}

#endif
