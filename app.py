from flask import Flask, render_template, Blueprint, request
import jwt
import sqlalchemy as db
import pymysql

app = Flask(__name__)

app.config['SECRET'] = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ=="
app.config['DB_HOST'] = "35.202.171.233"
app.config['DB_USER'] = "admin"
app.config['DB_PASS'] = "team6pass"
app.config['DB_NAME'] = "users"

#engine = db.create_engine('mysql+pymysql://' + app.config['DB_USER'] + ':' + app.config['DB_PASS'] + '@/' + app.config['DB_NAME'] + '/inlaid-goods-257500:us-central1:mysql-test', pool_pre_ping=True)

#app.config['DB_CONN'] = engine.connect()

@app.route('/')
def home():
    return render_template("obs_navigation.html")

@app.route('/signup', methods=["GET", "POST"])
def signup():

    if request.method == "GET":
        return render_template("signup.html")
    elif request.method == "POST":
        username = request.header.username
        password = request.header.password
        email = request.header.email
        

        #app.config['DB_CONN'].execute('INSERT INTO users VALUES(' + username + ', ' + password + ', ' + email + ')')

        #encoded = jwt.encode({'some': 'payload'}, app.config['SECRET'], algorithm='HS256')

        return "Success!"





@app.route('/welcome')
def welcome():
    return render_template("obs_home.html")

if __name__ == "__main__" :
    
    app.run(debug=True)