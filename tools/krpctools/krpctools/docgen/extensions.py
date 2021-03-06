import jinja2
import jinja2.ext

class AppendExtension(jinja2.ext.Extension):
    tags = set(['append'])

    def __init__(self, environment):
        super(AppendExtension, self).__init__(environment)

    def parse(self, parser):
        lineno = next(parser.stream).lineno
        args = [parser.parse_expression()]
        body = parser.parse_statements(['name:endappend'], drop_needle=True)
        return jinja2.nodes.CallBlock(self.call_method('_append_support', args), [], [], body).set_lineno(lineno)

    def _append_support(self, obj, caller):
        obj.append(caller())
        return ''
