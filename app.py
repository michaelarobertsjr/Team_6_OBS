from flask import Flask, render_template, Blueprint
import jwt
import sqlalchemy, pymysql

app = Flask(__name__)
app.config['SECRET'] = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ=="
app.config['DB_URL'] = ""

@app.route('/')
def home():
    return render_template("obs_navigation.html")

@app.route('/login')
def login():
    encoded = jwt.encode({'some': 'payload'}, app.config['SECRET'], algorithm='HS256')
    return render_template("obs_login.html")

@app.route('/welcome')
def welcome():
    return render_template("obs_home.html")

if __name__ == "__main__" :
    
    app.run()