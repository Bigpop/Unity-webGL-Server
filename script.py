from http.server import BaseHTTPRequestHandler, HTTPServer
import time
import os

PWD = os.path.dirname(os.path.realpath(__file__))

hostName = "localhost"
serverPort = 8080

class MyServer(BaseHTTPRequestHandler):
    mime_types = {
                    '.html': 'text/html', 
                    '.jpg':'image/jpg',
                    '.gif': 'image/gif',
                    '.js': 'application/javascript',
                    '.css': 'text/css',
                    '.png': 'image/png',
                    '.ico': 'image/ico',

                    '.js.br': 'application/javascript',
                    '.data.br': '',
                    '.wasm.br': 'application/wasm',
    }

    def do_GET(self):
        print(self.headers)
        try:
            file_path = PWD +'\\' +self.path[1:]
            print(file_path)
            for mime_type in self.mime_types.items():
                if self.path.endswith(mime_type[0]):
                    with open(file_path,'rb') as f:
                        self.send_response(200)
                        self.send_header('Content-type',mime_type[1])
                        # br压缩格式
                        if self.path.endswith('.br'):
                            self.send_header('Content-encoding','br')
                        self.end_headers()
                        self.wfile.write(f.read())

        except IOError:
            self.send_error(404,'File Not Found: %s' % self.path)

if __name__ == "__main__":  


    webServer = HTTPServer((hostName, serverPort), MyServer)
    print("Server started http://%s:%s" % (hostName, serverPort))
    print(PWD)

    try:
        webServer.serve_forever()
    except KeyboardInterrupt:
        pass

    webServer.server_close()
    print("Server stopped.")
