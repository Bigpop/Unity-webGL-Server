"""
    Unity webGL server for Linux
"""
from cmath import log
from http.server import BaseHTTPRequestHandler, HTTPServer
from re import S
import time
import os
import logging 


SYSTEM_TYPE = "Linux" # Linuix or Windows
PWD = os.path.dirname(os.path.realpath(__file__))


class Utility():

    @staticmethod
    def file_path(path):
        if SYSTEM_TYPE=='Linux':
            return  PWD  + path
        elif SYSTEM_TYPE=='Windows':
            return PWD +'\\' + path[1:]

    @staticmethod
    def check(path):
        if(path==None):
            return False;
        return os.path.isfile(path)


class MyServer(BaseHTTPRequestHandler):

    mime_types = {
                    '.html': 'text/html', 
                    '.jpg':'image/jpg',
                    '.gif': 'image/gif',
                    '.png': 'image/png',
                    '.ico': 'image/ico',
                    '.js': 'application/javascript',
                    '.css': 'text/css',

                    # Brotli compress
                    '.js.br': 'application/javascript',
                    '.data.br': '',
                    '.wasm.br': 'application/wasm',

                    # Gzip  compress
                    '.js.gz': 'application/javascript',
                    '.data.gz': '',
                    '.wasm.gz': 'application/wasm',
    }

    compress_types = {
        # Brotli compress
        '.gz':'gzip',
        # Gzip  compress
        '.br':'br',
    }

    def do_GET(self):

        file_path = Utility.file_path(self.path)
        # 先查看用户请求的文件是否存在
        if Utility.check(file_path)==False:
            logging.warning(f'File {file_path} Not Found')
            self.send_error(404,'File Not Found: %s' % self.path)
            return
        
    # Header
        self.send_response(200)
        # 返回媒体类型
        for mime_type in self.mime_types.items():
            if file_path.endswith(mime_type[0]):
                self.send_header('Content-type',mime_type[1])
        # 返回媒体压缩类型（如果有）
        for compress_type in self.compress_types.items():
            if file_path.endswith(compress_type[0]):
                logging.debug(compress_type,'返回压缩类型：', compress_type[1])
                self.send_header('Content-encoding',compress_type[1])
        self.end_headers()
        
    # Content
        with open(file_path, 'rb') as f:
            try:
                self.wfile.write(f.read())
            except Exception as e:
                #logging.error(e)
                pass

if __name__ == "__main__":  
    hostName = "" 
    serverPort = 8080
    webServer = HTTPServer((hostName, serverPort), MyServer)
    print("Server started http://%s:%s" % (hostName, serverPort))

    try:
        webServer.serve_forever()
    except KeyboardInterrupt:
        pass

    webServer.server_close()
    print("Server stopped.")
