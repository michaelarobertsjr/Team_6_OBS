from flask import Flask, render_template, Blueprint, request
import jwt
import sqlalchemy as db
import pymysql

app = Flask(__name__)

app.config['SECRET'] = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ=="
app.config['DB_HOST'] = "35.202.171.233"
app.config['DB_USER'] = "admin"
app.config['DB_PASS'] = "team6adminpass"
app.config['DB_NAME'] = "users"

engine = db.create_engine('mysql+pymysql://' + app.config['DB_USER'] + ':' + app.config['DB_PASS'] + '@' + app.config['DB_HOST'] + '/' + app.config['DB_NAME'], pool_pre_ping=True)
app.config['DB_CONN'] = engine.connect()

@app.route('/')
def home():
    return render_template("obs_navigation.html")

@app.route('/signup', methods=["GET", "POST"])
def signup():

    if request.method == "GET":
        return render_template("signup.html")
    elif request.method == "POST":
        username = request.form.get('username')
        assword = request.form.get("password")
        email = request.form.get("email")
        

        #app.config['DB_CONN'].execute('INSERT INTO accounts VALUES(' + username + ', ' + password + ', ' + email + ')')
        print(username)
        #encoded = jwt.encode({'some': 'payload'}, app.config['SECRET'], algorithm='HS256')
        return "Success!"

@app.route('/api/user', methods=["GET", "POST"])
def user():
    return "test"





@app.route('/welcome')
def welcome():
    return render_template("obs_home.html")

if __name__ == "__main__" :
    
    app.run(debug=True)